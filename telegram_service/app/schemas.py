from pydantic import BaseModel
from datetime import datetime


# === Admin ===
class AdminBase(BaseModel):
    telegram_id: str
    name: str | None = None


class AdminCreate(AdminBase):
    pass


class AdminUpdate(BaseModel):
    is_online: bool | None = None
    is_busy: bool | None = None
    name: str | None = None


class Admin(AdminBase):
    id: int
    is_online: bool
    is_busy: bool

    class Config:
        orm_mode = True


# === Message ===
class MessageBase(BaseModel):
    session_id: int
    sender: str  # 'user' or 'admin'
    content: str


class MessageCreate(MessageBase):
    pass


class Message(MessageBase):
    id: int
    timestamp: datetime

    class Config:
        orm_mode = True


# === Session ===
class SessionBase(BaseModel):
    user_id: str


class SessionCreate(BaseModel):
    user_name: str
    user_phone: str


class Session(SessionCreate):
    id: int
    admin_id: int
    admin_name: str  # ✅ For frontend display
    is_active: bool
    created_at: datetime

    class Config:
        orm_mode = True
