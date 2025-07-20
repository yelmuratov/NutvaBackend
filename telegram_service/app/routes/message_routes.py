from fastapi import APIRouter, Depends, HTTPException
from sqlalchemy.orm import Session
import os
import requests

from .. import crud, schemas, database, models
from ..connection_manager import manager

router = APIRouter(prefix="/messages", tags=["Messages"])


def get_db():
    db = database.SessionLocal()
    try:
        yield db
    finally:
        db.close()


@router.post("/start_session", response_model=schemas.Session)
def start_session(user: schemas.SessionCreate, db: Session = Depends(get_db)):
    session = crud.create_session_with_user(db, user)
    if not session:
        raise HTTPException(
            status_code=404,
            detail="No available admin at the moment. Please try again later."
        )

    admin = db.query(models.TelegramAdmin).filter_by(id=session.admin_id).first()
    session_dict = session.__dict__.copy()
    session_dict["admin_name"] = admin.name if admin and admin.name else "Support"

    # ✅ Send greeting to assigned Telegram admin via bot
    if admin and admin.telegram_id:
        bot_token = os.getenv("TELEGRAM_BOT_TOKEN")
        telegram_api_url = f"https://api.telegram.org/bot{bot_token}/sendMessage"
        try:
            greeting = (
                f"👋 Salom {admin.name}, sizga yangi foydalanuvchi biriktirildi.\n"
                f"🧑 Ismi: {user.user_name}\n📞 Telefon: {user.user_phone}\n\n"
            )
            response = requests.post(telegram_api_url, json={
                "chat_id": admin.telegram_id,
                "text": greeting
            })
            if not response.ok:
                print("❌ Failed to send greeting message to Telegram:", response.text)
        except Exception as e:
            print("❌ Error sending greeting message to Telegram:", e)
            
    return schemas.Session(**session_dict)

@router.post("/", response_model=schemas.Message)
async def post_message(message: schemas.MessageCreate, db: Session = Depends(get_db)):
    session = db.query(models.Session).filter_by(id=message.session_id).first()
    if not session or not session.is_active:
        raise HTTPException(status_code=400, detail="Session is closed or does not exist.")

    saved_message = crud.create_message(db, message)

    try:
        await manager.send_message(
            message.session_id,
            f"{message.sender}: {message.content}"
        )
    except Exception as e:
        print("⚠️ WebSocket broadcast failed:", e)

    if message.sender == "user" and session.admin_id:
        admin = db.query(models.TelegramAdmin).filter_by(id=session.admin_id).first()
        if admin and admin.telegram_id:
            bot_token = os.getenv("TELEGRAM_BOT_TOKEN")
            telegram_api_url = f"https://api.telegram.org/bot{bot_token}/sendMessage"
            try:
                response = requests.post(telegram_api_url, json={
                    "chat_id": admin.telegram_id,
                    "text": f"📩 Yangi xabar:\n{message.content}"
                })
                if not response.ok:
                    print("❌ Failed to send Telegram message:", response.text)
            except Exception as e:
                print("❌ Error sending Telegram message:", e)

    return saved_message


@router.get("/session/{session_id}", response_model=list[schemas.Message])
def get_conversation(session_id: int, db: Session = Depends(get_db)):
    return crud.get_session_messages(db, session_id)

@router.post("/end_session/{session_id}")
async def end_session(session_id: int, db: Session = Depends(get_db)):
    crud.close_session(db, session_id)
    try:
        await manager.send_message(session_id, "🔴 Session has ended.")
    except Exception as e:
        print("⚠️ WebSocket notify fail:", e)
    return {"success": True}


@router.get("/messages/session", response_model=list[schemas.Session])
def get_admin_active_sessions(admin_id: int, db: Session = Depends(get_db)):
    sessions = db.query(models.Session).filter_by(admin_id=admin_id, is_active=True).all()

    enriched_sessions = []
    for session in sessions:
        admin = db.query(models.TelegramAdmin).filter_by(id=session.admin_id).first()
        session_dict = session.__dict__.copy()
        session_dict["admin_name"] = admin.name if admin and admin.name else "Support"
        enriched_sessions.append(schemas.Session(**session_dict))

    return enriched_sessions
