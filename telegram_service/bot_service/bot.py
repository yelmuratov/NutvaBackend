from telegram import (
    Update,
    InlineKeyboardButton,
    InlineKeyboardMarkup,
    ReplyKeyboardMarkup,
    KeyboardButton,
)
from telegram.ext import (
    ApplicationBuilder,
    MessageHandler,
    CommandHandler,
    CallbackQueryHandler,
    filters,
    ContextTypes,
)
import requests
import os

BACKEND_URL = os.getenv("BACKEND_URL", "http://telegram-api:8000")
ADMIN_API_SECRET = os.getenv("ADMIN_API_SECRET", "default_token")
SUPER_ADMINS = [5766282037, 8003316266]

# Helper function to get headers for admin API requests
def get_admin_headers():
    return {"x-admin-secret": ADMIN_API_SECRET}


def get_admin_by_telegram_id(telegram_id: str):
    try:
        resp = requests.get(f"{BACKEND_URL}/admins/", headers=get_admin_headers())
        resp.raise_for_status()
        admins = resp.json()
        return next((a for a in admins if a["telegram_id"] == telegram_id), None)
    except Exception as e:
        print("❌ Error checking admin:", e)
        return None

# === Enhanced Keyboard Functions ===
def build_status_keyboard(is_online: bool):
    return InlineKeyboardMarkup(
        [[
            InlineKeyboardButton(
                "🔴 Go Offline" if is_online else "🟢 Go Online",
                callback_data="offline" if is_online else "online"
            )
        ]]
    )

def build_finish_session_keyboard(session_id: int):
    return InlineKeyboardMarkup(
        [[InlineKeyboardButton("✅ Finish Session", callback_data=f"end_{session_id}")]]
    )

def build_admin_management_keyboard():
    """Build keyboard for super admin management functions"""
    keyboard = [
        [
            InlineKeyboardButton("➕ Add Admin", callback_data="add_admin_prompt"),
            InlineKeyboardButton("🗑️ Delete Admin", callback_data="delete_admin_prompt")
        ],
        [
            InlineKeyboardButton("📋 List Admins", callback_data="list_admins"),
            InlineKeyboardButton("🔙 Back", callback_data="back_to_main")
        ]
    ]
    return InlineKeyboardMarkup(keyboard)

def build_admin_list_keyboard(admins):
    """Build keyboard showing admin list with delete options"""
    keyboard = []
    for admin in admins:
        keyboard.append([
            InlineKeyboardButton(
                f"🗑️ {admin['name']} (ID: {admin['telegram_id']})", 
                callback_data=f"delete_admin_{admin['id']}"
            )
        ])
    keyboard.append([InlineKeyboardButton("🔙 Back", callback_data="admin_management")])
    return InlineKeyboardMarkup(keyboard)

def main_reply_keyboard():
    return ReplyKeyboardMarkup(
        [[KeyboardButton("🟢 Status"), KeyboardButton("📌 Start Panel")]],
        resize_keyboard=True,
        one_time_keyboard=False
    )

def super_admin_reply_keyboard():
    """Enhanced keyboard for super admins"""
    return ReplyKeyboardMarkup(
        [
            [KeyboardButton("🟢 Status"), KeyboardButton("📌 Start Panel")],
            [KeyboardButton("⚙️ Admin Management")]
        ],
        resize_keyboard=True,
        one_time_keyboard=False
    )

# === Command Handlers ===
async def start_command(update: Update, context: ContextTypes.DEFAULT_TYPE):
    telegram_id = str(update.effective_user.id)
    admin = get_admin_by_telegram_id(telegram_id)

    if not admin:
        await update.message.reply_text("🚫 You are not registered as an admin.")
        return

    # Check if user is super admin
    keyboard = super_admin_reply_keyboard() if update.effective_user.id in SUPER_ADMINS else main_reply_keyboard()
    
    msg = await update.message.reply_text(
        f"👋 Salom {admin['name']}! Pastki menyudan foydalaning:",
        reply_markup=keyboard
    )
    context.user_data.setdefault("bot_messages", []).append(msg.message_id)

# === Text Menu Handler ===
async def handle_text_commands(update: Update, context: ContextTypes.DEFAULT_TYPE):
    telegram_id = str(update.effective_user.id)
    admin = get_admin_by_telegram_id(telegram_id)
    if not admin:
        await update.message.reply_text("🚫 You are not registered as an admin.")
        return

    msg = update.message.text.lower().strip()

    if msg == "🟢 status":
        status_msg = await update.message.reply_text(
            "Holatingizni tanlang:",
            reply_markup=build_status_keyboard(admin["is_online"])
        )
        context.user_data.setdefault("bot_messages", []).append(status_msg.message_id)

    elif msg == "📌 start panel":
        keyboard = super_admin_reply_keyboard() if update.effective_user.id in SUPER_ADMINS else main_reply_keyboard()
        panel_msg = await update.message.reply_text(
            f"👋 Xush kelibsiz {admin['name']}!",
            reply_markup=keyboard
        )
        context.user_data.setdefault("bot_messages", []).append(panel_msg.message_id)

    elif msg == "⚙️ admin management":
        if update.effective_user.id not in SUPER_ADMINS:
            await update.message.reply_text("🚫 You are not authorized to access admin management.")
            return
        
        mgmt_msg = await update.message.reply_text(
            "⚙️ Admin Management Panel\nTanlang:",
            reply_markup=build_admin_management_keyboard()
        )
        context.user_data.setdefault("bot_messages", []).append(mgmt_msg.message_id)

    else:
        await handle_admin_message(update, context)

async def handle_admin_message(update: Update, context: ContextTypes.DEFAULT_TYPE):
    telegram_id = str(update.effective_user.id)
    text = update.message.text.strip()

    admin = get_admin_by_telegram_id(telegram_id)
    if not admin:
        await update.message.reply_text("🚫 You are not registered as an admin.")
        return

    # Handle add admin process
    if context.user_data.get("waiting_for_admin_data"):
        await handle_add_admin_input(update, context, text)
        return

    try:
        session_resp = requests.get(
            f"{BACKEND_URL}/messages/messages/session", params={"admin_id": admin["id"]}
        )
        session_resp.raise_for_status()
        active_sessions = session_resp.json()
    except Exception as e:
        print(f"❌ Error fetching session: {e}")
        await update.message.reply_text("❌ Failed to get session.")
        return

    if not active_sessions:
        await update.message.reply_text("❌ No active session found.")
        return

    session = active_sessions[0]

    try:
        messages_resp = requests.get(
            f"{BACKEND_URL}/messages/", params={"session_id": session["id"]}
        )
        messages_resp.raise_for_status()
        messages = messages_resp.json()
    except Exception as e:
        print(f"❌ Error fetching messages: {e}")

    try:
        send_resp = requests.post(f"{BACKEND_URL}/messages/", json={
            "session_id": session["id"],
            "sender": "admin",
            "content": text
        })
        send_resp.raise_for_status()
    except Exception as e:
        print(f"❌ Error posting message: {e}")
        await update.message.reply_text("❌ Failed to send message.")
        return

    sent_msg = await update.message.reply_text(
        "✅ Xabar foydalanuvchiga yuborildi.",
        reply_markup=build_finish_session_keyboard(session["id"])
    )

    context.user_data.setdefault("bot_messages", []).append(sent_msg.message_id)

# === New Admin Management Functions ===
async def handle_add_admin_input(update: Update, context: ContextTypes.DEFAULT_TYPE, text: str):
    """Handle the add admin input process"""
    try:
        parts = text.strip().split()
        if len(parts) != 2:
            await update.message.reply_text(
                "❌ Noto'g'ri format. Iltimos, faqat ism va telegram ID kiriting:\nMisol: John 123456789"
            )
            return

        name, telegram_id = parts
        
        resp = requests.post(f"{BACKEND_URL}/admins/", 
                           json={
                               "name": name,
                               "telegram_id": telegram_id
                           },
                           headers=get_admin_headers())
        resp.raise_for_status()
        
        await update.message.reply_text(
            f"✅ Admin muvaffaqiyatli qo'shildi:\n👤 Ism: {name}\n🆔 Telegram ID: {telegram_id}",
            reply_markup=build_admin_management_keyboard()
        )
        
        # Clear the waiting state
        context.user_data["waiting_for_admin_data"] = False
        
    except Exception as e:
        await update.message.reply_text(f"❌ Admin qo'shishda xatolik: {e}")
        context.user_data["waiting_for_admin_data"] = False

# === Enhanced Callback Handler ===
async def handle_callback(update: Update, context: ContextTypes.DEFAULT_TYPE):
    query = update.callback_query
    await query.answer()
    telegram_id = str(query.from_user.id)
    data = query.data

    admin = get_admin_by_telegram_id(telegram_id)
    if not admin:
        await query.edit_message_text("🚫 Siz administrator sifatida ro'yxatdan o'tmagansiz.")
        return

    # Handle admin management callbacks
    if data == "admin_management":
        if query.from_user.id not in SUPER_ADMINS:
            await query.edit_message_text("🚫 Sizda admin boshqaruvi uchun ruxsat yo'q.")
            return
        
        await query.edit_message_text(
            "⚙️ Admin Management Panel\nTanlang:",
            reply_markup=build_admin_management_keyboard()
        )
        return

    elif data == "add_admin_prompt":
        if query.from_user.id not in SUPER_ADMINS:
            await query.edit_message_text("🚫 Sizda admin qo'shish uchun ruxsat yo'q.")
            return
        
        context.user_data["waiting_for_admin_data"] = True
        await query.edit_message_text(
            "➕ Yangi admin qo'shish\n\nIltimos, admin ma'lumotlarini quyidagi formatda kiriting:\n\n<ism> <telegram_id>\n\nMisol: John 123456789",
            reply_markup=InlineKeyboardMarkup([[InlineKeyboardButton("🔙 Bekor qilish", callback_data="admin_management")]])
        )
        return

    elif data == "delete_admin_prompt":
        if query.from_user.id not in SUPER_ADMINS:
            await query.edit_message_text("🚫 Sizda admin o'chirish uchun ruxsat yo'q.")
            return
        
        try:
            resp = requests.get(f"{BACKEND_URL}/admins/", headers=get_admin_headers())
            resp.raise_for_status()
            admins = resp.json()
            
            if not admins:
                await query.edit_message_text(
                    "📭 Hech qanday admin topilmadi.",
                    reply_markup=InlineKeyboardMarkup([[InlineKeyboardButton("🔙 Back", callback_data="admin_management")]])
                )
                return
            
            await query.edit_message_text(
                "🗑️ O'chirish uchun adminni tanlang:",
                reply_markup=build_admin_list_keyboard(admins)
            )
        except Exception as e:
            await query.edit_message_text(f"❌ Adminlar ro'yxatini olishda xatolik: {e}")
        return

    elif data.startswith("delete_admin_"):
        if query.from_user.id not in SUPER_ADMINS:
            await query.edit_message_text("🚫 Sizda admin o'chirish uchun ruxsat yo'q.")
            return
        
        admin_id_to_delete = data.replace("delete_admin_", "")
        try:
            # Get admin info first to show confirmation
            resp = requests.get(f"{BACKEND_URL}/admins/", headers=get_admin_headers())
            resp.raise_for_status()
            admins = resp.json()
            admin_to_delete = next((a for a in admins if str(a["id"]) == admin_id_to_delete), None)
            
            if not admin_to_delete:
                await query.edit_message_text(
                    "❌ Admin topilmadi.",
                    reply_markup=build_admin_management_keyboard()
                )
                return
            
            # Delete the admin using admin_id
            resp = requests.delete(f"{BACKEND_URL}/admins/{admin_id_to_delete}", headers=get_admin_headers())
            resp.raise_for_status()
            await query.edit_message_text(
                f"🗑️ Admin muvaffaqiyatli o'chirildi:\n👤 {admin_to_delete['name']}\n🆔 Telegram ID: {admin_to_delete['telegram_id']}",
                reply_markup=build_admin_management_keyboard()
            )
        except Exception as e:
            await query.edit_message_text(
                f"❌ Admin o'chirishda xatolik: {e}",
                reply_markup=build_admin_management_keyboard()
            )
        return

    elif data == "list_admins":
        if query.from_user.id not in SUPER_ADMINS:
            await query.edit_message_text("🚫 Sizda adminlar ro'yxatini ko'rish uchun ruxsat yo'q.")
            return
        
        try:
            resp = requests.get(f"{BACKEND_URL}/admins/", headers=get_admin_headers())
            resp.raise_for_status()
            admins = resp.json()
            
            if not admins:
                text = "📭 Hech qanday admin topilmadi."
            else:
                text = "📋 Adminlar ro'yxati:\n\n"
                for i, admin_item in enumerate(admins, 1):
                    status = "🟢" if admin_item.get("is_online", False) else "🔴"
                    text += f"{i}. {status} {admin_item['name']}\n   🆔 ID: {admin_item['telegram_id']}\n\n"
            
            await query.edit_message_text(
                text,
                reply_markup=InlineKeyboardMarkup([[InlineKeyboardButton("🔙 Back", callback_data="admin_management")]])
            )
        except Exception as e:
            await query.edit_message_text(f"❌ Adminlar ro'yxatini olishda xatolik: {e}")
        return

    elif data == "back_to_main":
        keyboard = super_admin_reply_keyboard() if query.from_user.id in SUPER_ADMINS else main_reply_keyboard()
        await query.edit_message_text(
            f"👋 Xush kelibsiz {admin['name']}!",
            reply_markup=InlineKeyboardMarkup([[InlineKeyboardButton("🔙 Menu", callback_data="show_menu")]])
        )
        return

    # Handle session management for status changes
    try:
        session_resp = requests.get(
            f"{BACKEND_URL}/messages/messages/session", 
            params={"admin_id": admin["id"]},
            headers=get_admin_headers()  # Add headers here too for consistency
        )
        session_resp.raise_for_status()
        active_sessions = session_resp.json()
    except Exception as e:
        print(f"❌ Session check error: {e}")
        await query.edit_message_text("❌ Sessiyani tekshirib bo'lmadi.")
        return

    if data.startswith("end_"):
        session_id = int(data.replace("end_", ""))
        try:
            resp = requests.post(f"{BACKEND_URL}/messages/end_session/{session_id}", headers=get_admin_headers())
            resp.raise_for_status()

            try:
                await query.edit_message_text("✅ Sessiya yakunlandi. Yangi foydalanuvchini qabul qilishingiz mumkin.")
            except Exception as e:
                print("⚠️ Could not edit finish button message:", e)

            # Delete all stored bot messages
            for msg_id in context.user_data.get("bot_messages", []):
                try:
                    await context.bot.delete_message(chat_id=query.from_user.id, message_id=msg_id)
                except Exception as e:
                    print(f"⚠️ Message delete failed: {e}")
            context.user_data["bot_messages"] = []

            keyboard = super_admin_reply_keyboard() if query.from_user.id in SUPER_ADMINS else main_reply_keyboard()
            await context.bot.send_message(
                chat_id=query.from_user.id,
                text="📭 Sessiya tugadi. Pastki menyudan davom eting.",
                reply_markup=keyboard
            )

        except Exception as e:
            print("❌ Sessiyani tugatishda xatolik:", e)
            await query.edit_message_text("❌ Sessiyani tugatib bo'lmadi.")

    elif active_sessions:
        await query.edit_message_text(
            "⚠️ Avval mavjud sessiyani yakunlang. Keyin holatingizni o'zgartirishingiz mumkin."
        )
    else:
        # STATUS UPDATE SECTION - THIS IS THE FIX
        is_online = data == "online"
        try:
            # Create the complete admin update payload
            # Based on your API schema, you need to send all required fields
            update_data = {
                "telegram_id": admin["telegram_id"],  # Required field
                "name": admin["name"],                # Required field
                "is_online": is_online,               # The field you want to change
                "is_busy": admin.get("is_busy", False)  # Include existing is_busy value
            }
            
            response = requests.put(
                f"{BACKEND_URL}/admins/{admin['id']}",
                json=update_data,
                headers={
                    "Content-Type": "application/json",  # Explicitly set content type
                    **get_admin_headers()  # Include admin authentication headers
                }
            )
            response.raise_for_status()

            new_markup = build_status_keyboard(is_online)
            await query.edit_message_text(
                f"✅ Holat yangilandi: {'Online 🟢' if is_online else 'Offline 🔴'}",
                reply_markup=new_markup
            )
        except requests.exceptions.HTTPError as e:
            print(f"❌ HTTP Error updating status: {e}")
            print(f"❌ Response content: {e.response.text if e.response else 'No response'}")
            await query.edit_message_text("❌ Holatni yangilab bo'lmadi. Server xatosi.")
        except Exception as e:
            print("❌ Holatni yangilashda xatolik:", e)
            await query.edit_message_text("❌ Holatni yangilab bo'lmadi.")

# === Legacy Command Handlers (kept for compatibility) ===
async def add_admin(update: Update, context: ContextTypes.DEFAULT_TYPE):
    if update.effective_user.id not in SUPER_ADMINS:
        await update.message.reply_text("🚫 You are not authorized to use this command.")
        return

    try:
        name = context.args[0]
        telegram_id = context.args[1]
        resp = requests.post(f"{BACKEND_URL}/admins/", 
                           json={
                               "name": name,
                               "telegram_id": telegram_id
                           },
                           headers=get_admin_headers())
        resp.raise_for_status()
        await update.message.reply_text(f"✅ Admin added: {name} (ID: {telegram_id})")
    except Exception as e:
        await update.message.reply_text(f"❌ Error adding admin: {e}")

async def delete_admin(update: Update, context: ContextTypes.DEFAULT_TYPE):
    if update.effective_user.id not in SUPER_ADMINS:
        await update.message.reply_text("🚫 You are not authorized to use this command.")
        return

    try:
        telegram_id = context.args[0]
        
        # First get all admins to find the admin_id
        resp = requests.get(f"{BACKEND_URL}/admins/", headers=get_admin_headers())
        resp.raise_for_status()
        admins = resp.json()
        admin_to_delete = next((a for a in admins if a["telegram_id"] == telegram_id), None)
        
        if not admin_to_delete:
            await update.message.reply_text(f"❌ Admin with Telegram ID {telegram_id} not found.")
            return
        
        # Delete using admin_id
        resp = requests.delete(f"{BACKEND_URL}/admins/{admin_to_delete['id']}", headers=get_admin_headers())
        resp.raise_for_status()
        await update.message.reply_text(f"🗑️ Admin deleted: {admin_to_delete['name']} (Telegram ID: {telegram_id})")
    except Exception as e:
        await update.message.reply_text(f"❌ Error deleting admin: {e}")

async def list_admins(update: Update, context: ContextTypes.DEFAULT_TYPE):
    if update.effective_user.id not in SUPER_ADMINS:
        await update.message.reply_text("🚫 You are not authorized to use this command.")
        return

    try:
        resp = requests.get(f"{BACKEND_URL}/admins/", headers=get_admin_headers())
        resp.raise_for_status()
        admins = resp.json()
        text = "\n".join([f"{a['name']} (ID: {a['telegram_id']})" for a in admins])
        await update.message.reply_text(f"📋 Admins:\n{text}")
    except Exception as e:
        await update.message.reply_text(f"❌ Error listing admins: {e}")

# === Run the bot ===
def run_bot():
    app = ApplicationBuilder().token(os.getenv("TELEGRAM_BOT_TOKEN")).build()
    app.add_handler(CommandHandler("start", start_command))
    app.add_handler(CallbackQueryHandler(handle_callback))
    app.add_handler(MessageHandler(filters.TEXT & (~filters.COMMAND), handle_text_commands))
    
    # Keep legacy commands for compatibility
    app.add_handler(CommandHandler("add_admin", add_admin))
    app.add_handler(CommandHandler("delete_admin", delete_admin))
    app.add_handler(CommandHandler("list_admins", list_admins))

    print("✅ Telegram bot is running...")
    app.run_polling()

if __name__ == "__main__":
    run_bot()