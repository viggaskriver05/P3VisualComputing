from pythonosc import dispatcher
from pythonosc import osc_server

def emotion_handler(address, *args):
    """Handle incoming emotion messages"""
    print(f"Received on {address}: {args[0] if args else 'No data'}")

if __name__ == "__main__":
    # Create dispatcher to handle incoming messages
    disp = dispatcher.Dispatcher()
    disp.map("/emotion", emotion_handler)
    
    # Set up server
    IP = "127.0.0.1"
    PORT = 9000
    
    server = osc_server.ThreadingOSCUDPServer((IP, PORT), disp)
    
    print(f"Listening for OSC messages on {IP}:{PORT}")
    print(f"Waiting for /emotion messages...")
    print("Press Ctrl+C to stop\n")
    
    try:
        server.serve_forever()
    except KeyboardInterrupt:
        print("\nServer stopped")
        server.shutdown()
