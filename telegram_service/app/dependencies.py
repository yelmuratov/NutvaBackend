import os
from fastapi import Header, HTTPException

ADMIN_API_SECRET = os.getenv("ADMIN_API_SECRET", "default_token")

def verify_admin_api_token(x_admin_secret: str = Header(...)):
    if x_admin_secret != ADMIN_API_SECRET:
        raise HTTPException(status_code=403, detail="Forbidden: Invalid token" + x_admin_secret + ADMIN_API_SECRET)
