from sqlalchemy import Column, Integer, String, Boolean, ForeignKey, DateTime, Text
from sqlalchemy.orm import relationship
from datetime import datetime
from .database import Base

class TelegramAdmin(Base):
    __tablename__ = "telegram_admins"

    id = Column(Integer, primary_key=True, index=True)
    telegram_id = Column(String, unique=True, index=True, nullable=False)
    name = Column(String, nullable=True)
    is_online = Column(Boolean, default=False)
    is_busy = Column(Boolean, default=False)

class Session(Base):
    __tablename__ = "sessions"

    id = Column(Integer, primary_key=True)
    user_name = Column(String, nullable=False)
    user_phone = Column(String, nullable=False)
    admin_id = Column(Integer, ForeignKey("telegram_admins.id"))
    is_active = Column(Boolean, default=True)
    created_at = Column(DateTime, default=datetime.utcnow)
    greeting_message_id = Column(Integer, nullable=True)  # ✅ Add this line

    admin = relationship("TelegramAdmin", backref="sessions")

class Message(Base):
    __tablename__ = "messages"

    id = Column(Integer, primary_key=True)
    session_id = Column(Integer, ForeignKey("sessions.id"))
    sender = Column(String)  # 'user' or 'admin'
    content = Column(Text)
    timestamp = Column(DateTime, default=datetime.utcnow)

    session = relationship("Session", backref="messages")