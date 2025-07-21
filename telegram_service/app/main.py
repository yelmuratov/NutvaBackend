from fastapi import FastAPI, WebSocket, WebSocketDisconnect
from fastapi.middleware.cors import CORSMiddleware
from .database import Base, engine
from .routes import admin_routes, message_routes
from .connection_manager import manager  # ✅ Avoid circular import

# ✅ Initialize DB tables
Base.metadata.create_all(bind=engine)

# ✅ Create FastAPI app with correct root_path
app = FastAPI(root_path="/telegram-api")

# ✅ Enable CORS for frontend access (adjust as needed)
app.add_middleware(
    CORSMiddleware,
    allow_origins=[
        "https://nutvahealth.uz",
        "https://www.nutvahealth.uz",
        "https://demo.nutva.uz",
        "https://www.demo.nutva.uz",
        "http://localhost:3000",
        "https://nutva-frontend.vercel.app",
    ],
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"],
)

# ✅ Register routers
app.include_router(admin_routes.router)
app.include_router(message_routes.router)

# ✅ WebSocket endpoint for real-time chat
@app.websocket("/ws/{session_id}")
async def websocket_endpoint(websocket: WebSocket, session_id: int):
    await manager.connect(session_id, websocket)
    try:
        while True:
            await websocket.receive_text()  # Placeholder: Heartbeat or ping
    except WebSocketDisconnect:
        manager.disconnect(session_id, websocket)
