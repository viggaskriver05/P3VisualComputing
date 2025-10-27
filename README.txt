Dependencies included in requirements.txt:
deepface==0.0.93
opencv-python==4.10.0.84
python-osc==1.8.3
tf-keras==2.20.1

navigate to root source in terminal

create new virtual enviroment:
python -m venv .venv

activate virtual enviroment through terminal or when prompted in VS/code
source .venv/bin/activate        # macOS/Linux
.venv\Scripts\activate           # Windows

install requirements:
pip install -r requirements.txt

run deepface client which includes OSC transmitter:
python deepface_client.py

close deepface_client: 
ESC

deactivate virtual envirment: 
deactivate

