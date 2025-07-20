from fastapi import APIRouter, Depends, HTTPException
from sqlalchemy.orm import Session
from .. import crud, schemas, database
from ..dependencies import verify_admin_api_token

router = APIRouter(
    prefix="/admins",
    tags=["Admins"],
    dependencies=[Depends(verify_admin_api_token)]  # 🔐 Apply to all endpoints in this router
)

def get_db():
    db = database.SessionLocal()
    try:
        yield db
    finally:
        db.close()

@router.get("/", response_model=list[schemas.Admin])
def list_admins(db: Session = Depends(get_db)):
    return crud.get_admins(db)

@router.post("/", response_model=schemas.Admin)
def add_admin(admin: schemas.AdminCreate, db: Session = Depends(get_db)):
    return crud.create_admin(db, admin)

@router.put("/{admin_id}", response_model=schemas.Admin)
def modify_admin(admin_id: int, admin: schemas.AdminUpdate, db: Session = Depends(get_db)):
    return crud.update_admin(db, admin_id, admin)

@router.delete("/{admin_id}")
def remove_admin(admin_id: int, db: Session = Depends(get_db)):
    return {"success": crud.delete_admin(db, admin_id)}

@router.delete("/by_telegram_id/{telegram_id}")
def remove_admin_by_telegram_id(telegram_id: str, db: Session = Depends(get_db)):
    admin = next((a for a in crud.get_admins(db) if str(a.telegram_id) == telegram_id), None)
    if not admin:
        raise HTTPException(status_code=404, detail="Admin not found")
    return {"success": crud.delete_admin(db, admin.id)}
