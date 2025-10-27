from deepface import DeepFace
import cv2
import threading
from pythonosc.udp_client import SimpleUDPClient
import time

#OSC SENDER TO UNITY
UNITY_IP = "127.0.0.1"
UNITY_PORT = 9000    
ADDRESS = "/emotion" 

client = SimpleUDPClient(UNITY_IP, UNITY_PORT)

#cv2 FER analysis
cap = cv2.VideoCapture(0)
frame_count = 0
text = '' 
analysis_lock = threading.Lock()
latest_frame = None

# Sending rate control for OSC messages
last_send_time = time.time()
SEND_HZ = 10
send_interval = 1.0 / SEND_HZ

def analyze_frame(frame):
    global text
    try:
        result = DeepFace.analyze(
            frame,
            actions=['emotion'],
            detector_backend='opencv',
            enforce_detection=True
        )
        emo = result[0].get('dominant_emotion', '')
        conf = result[0].get('emotion', {}).get(emo, None)
        new_text = f'Emotion: {emo}'
        if conf is not None:
            new_text += f' ({conf:.2f})'
    except Exception:
        new_text = 'No face detected'
    with analysis_lock:
        text = new_text

analysis_thread = None
while True:
    ok, frame = cap.read()
    if not ok:
        break

    frame_count += 1
    if frame_count % 1 == 0:
        # Start analysis in a separate thread if not already running
        if analysis_thread is None or not analysis_thread.is_alive():
            analysis_thread = threading.Thread(target=analyze_frame, args=(frame.copy(),))
            analysis_thread.start()

    # Send emotion to Unity at specified rate
    current_time = time.time()
    if current_time - last_send_time >= send_interval:
        with analysis_lock:
            emotion_text = text
        # Extract just the emotion name from the text (e.g., "happy" from "Emotion: happy (0.95)")
        if emotion_text and emotion_text != 'No face detected':
            emotion_label = emotion_text.split(':')[1].split('(')[0].strip()
        else:
            emotion_label = 'neutral'
        client.send_message(ADDRESS, emotion_label)
        last_send_time = current_time

    with analysis_lock:
        display_text = text
    cv2.putText(frame, display_text, (10, 30), cv2.FONT_HERSHEY_SIMPLEX, 1, (255, 255, 255), 2)
    cv2.imshow('emo', frame)
    if cv2.waitKey(1) & 0xFF == 27:
        break
cap.release()
cv2.destroyAllWindows()