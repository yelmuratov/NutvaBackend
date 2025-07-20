from fastapi import FastAPI, WebSocket, WebSocketDisconnect
from fastapi.middleware.cors import CORSMiddleware
from .database import Base, engine
from .routes import admin_routes, message_routes
from .connection_manager import manager  # ✅ Avoid circular import

# Initialize DB tables
Base.metadata.create_all(bind=engine)

# Create FastAPI app
app = FastAPI()

# ✅ Enable CORS for frontend access (e.g. http://127.0.0.1:5500)
app.add_middleware(
    CORSMiddleware,
    allow_origins=["*"],  # You can limit to ["http://127.0.0.1:5500"] for tighter control
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"],
)

# Register routers
app.include_router(admin_routes.router)
app.include_router(message_routes.router)

# WebSocket endpoint
@app.websocket("/ws/{session_id}")
async def websocket_endpoint(websocket: WebSocket, session_id: int):
    await manager.connect(session_id, websocket)
    try:
        while True:
            await websocket.receive_text()  # Placeholder: Heartbeat or incoming ping
    except WebSocketDisconnect:
        manager.disconnect(session_id, websocket)
