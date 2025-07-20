from .models import Session as ChatSession, Message, TelegramAdmin
from sqlalchemy.orm import Session
from . import models, schemas


# === SESSION LOGIC ===

def create_session_with_user(db: Session, user_data: schemas.SessionCreate):
    # Check if user already has an active session
    existing = db.query(models.Session).filter_by(
        user_phone=user_data.user_phone,
        is_active=True
    ).first()
    if existing:
        return existing

    # Find available admin (online and not busy)
    admin = db.query(models.TelegramAdmin).filter_by(
        is_online=True,
        is_busy=False
    ).first()

    if not admin:
        return None

    # Assign and set admin busy
    admin.is_busy = True
    session = models.Session(
        user_name=user_data.user_name,
        user_phone=user_data.user_phone,
        admin_id=admin.id,
        is_active=True
    )
    db.add(session)
    db.commit()
    db.refresh(session)
    return session


def close_session(db: Session, session_id: int):
    session = db.query(ChatSession).get(session_id)
    if session and session.is_active:
        session.is_active = False
        admin = db.query(TelegramAdmin).get(session.admin_id)
        if admin:
            admin.is_busy = False  # Free the admin
        db.commit()


def create_message(db: Session, msg_data: schemas.MessageCreate):
    msg = Message(**msg_data.dict())
    db.add(msg)
    db.commit()
    db.refresh(msg)
    return msg


def get_session_messages(db: Session, session_id: int):
    return db.query(Message).filter_by(session_id=session_id).order_by(Message.timestamp).all()


# === ADMIN LOGIC ===

def get_admins(db: Session):
    return db.query(TelegramAdmin).all()


def create_admin(db: Session, admin: schemas.AdminCreate):
    db_admin = TelegramAdmin(**admin.dict())
    db.add(db_admin)
    db.commit()
    db.refresh(db_admin)
    return db_admin


def update_admin(db: Session, admin_id: int, update_data: schemas.AdminUpdate):
    db_admin = db.query(TelegramAdmin).get(admin_id)
    if db_admin is None:
        return None
    for key, value in update_data.dict(exclude_unset=True).items():
        setattr(db_admin, key, value)
    db.commit()
    db.refresh(db_admin)
    return db_admin


def delete_admin(db: Session, admin_id: int):
    db_admin = db.query(TelegramAdmin).get(admin_id)
    if db_admin:
        db.delete(db_admin)
        db.commit()
    return True
