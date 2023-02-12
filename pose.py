import cv2
import mediapipe as mp
import socket
import json
from socket import *

# UDP receiver ip & port

UDP_IP_3 = "127.0.0.1"
UDP_PORT_3 = 10001 # Unity Section 1 Camera
UDP_PORT_4 = 20003 # Max Section 1 Camera

UDP_IP_5 = "10.137.19.131" # Ethernet Shield 1 for small mushroom light show
UDP_PORT_5 = 10050 # Small Mushroom Light Show

UDP_IP_6 = "10.137.19.140" # Ethernet Shield 2 for Glowing Eyes Owl and funky tree lights
UDP_PORT_6 = 15001 # Glowing Eyes Owl

SERVER_PORT = 20030
SERVER_IP = "10.137.19.141"

data = {
    "hw" : {
        "yR" : 0,
        "st" : 0
    }
}

intrusion = {"intrusion" : 0}

mp_drawing = mp.solutions.drawing_utils
mp_pose = mp.solutions.pose

def highestWrist(leftWrist, rightWrist, rightShoulder, leftShoulder, rightHip, leftHip):
    sternum = rightShoulder + leftShoulder
    sternum = (sternum / 2)
    hip = rightHip + leftHip
    hip = (hip / 2)
    yMax = min(leftWrist, rightWrist)
    div = abs(sternum - hip)
    yRel = ((sternum - yMax) / div)
    if yRel > 1:
        yRel = 1
    if yRel < -1:
        yRel = -1
    yRel = "{:.2f}".format(yRel)
    yRel = float(yRel)
    return yRel

sock = socket(AF_INET, SOCK_DGRAM) # UDP
yRel = 0

cap = cv2.VideoCapture(0)
with mp_pose.Pose(min_detection_confidence=0.5, min_tracking_confidence=0.5) as pose:
#    serverSocket = socket(AF_INET, SOCK_DGRAM)
#    serverSocket.bind((SERVER_IP, SERVER_PORT))

    while cap.isOpened():
        # data = serverSocket.recv(1024)
        # intrusion = json.loads(data.decode())
        ret, frame = cap.read()
        # Recolor image to RGB
        image = cv2.cvtColor(frame, cv2.COLOR_BGR2RGB)
        image.flags.writeable = False

        # Make detection
        results = pose.process(image)

        # Recolor back to BGR
        image.flags.writeable = True
        image = cv2.cvtColor(image, cv2.COLOR_RGB2BGR)

        # Extract landmarks
        try:
            landmarks = results.pose_landmarks.landmark

            # Get coordinates
            leftWrist = landmarks[mp_pose.PoseLandmark.LEFT_WRIST].y
            rightWrist = landmarks[mp_pose.PoseLandmark.RIGHT_WRIST].y
            rightShoulder = landmarks[mp_pose.PoseLandmark.RIGHT_SHOULDER].y
            leftShoulder = landmarks[mp_pose.PoseLandmark.LEFT_SHOULDER].y
            rightHip = landmarks[mp_pose.PoseLandmark.RIGHT_HIP].y
            leftHip = landmarks[mp_pose.PoseLandmark.LEFT_HIP].y



            yRel = highestWrist(leftWrist, rightWrist, rightShoulder, leftShoulder, rightHip, leftHip)
            # print(yRel)
        # Send UDP message to target
            #data["highest_wrist"]["yRel"] = yRel
            #MESSAGE = bytes(json.dumps(data), "ascii")
            #print(MESSAGE)
            #problem might be in this line?
            data = {
                "h1" : yRel,
                "intrusion" : intrusion["intrusion"]
            }

            data = bytes(json.dumps(data), 'UTF-8')
            #sock.sendto(data, (UDP_IP, UDP_PORT))  

            sock.sendto(data, (UDP_IP_3, UDP_PORT_3)) 
#            sock.sendto(data, (UDP_IP_3, UDP_PORT_4))
#            sock.sendto(data, (UDP_IP_5, UDP_PORT_5))  
#            sock.sendto(data, (UDP_IP_6, UDP_PORT_6))  
        except:
            yRel = -1
            data = {
                "h1" : yRel,
                "intrusion" : intrusion["intrusion"]
            }
           

            data = bytes(json.dumps(data), 'UTF-8')
            #sock.sendto(data, (UDP_IP, UDP_PORT))  

            sock.sendto(data, (UDP_IP_3, UDP_PORT_3)) 
#            sock.sendto(data, (UDP_IP_3, UDP_PORT_4)) 
#            sock.sendto(data, (UDP_IP_5, UDP_PORT_5)) 
#            sock.sendto(data, (UDP_IP_6, UDP_PORT_6))
            pass

        # display info
        cv2.rectangle(image, (0,0), (255, 73), (245,117,16), -1)
        cv2.putText(image, str(yRel),
                (10, 60),
                cv2.FONT_HERSHEY_SIMPLEX, 2, (255,255,255), 2, cv2.LINE_AA)

        # Render detections
        mp_drawing.draw_landmarks(image, results.pose_landmarks, mp_pose.POSE_CONNECTIONS,
        mp_drawing.DrawingSpec(color=(245,117,66), thickness=2, circle_radius=2),
        mp_drawing.DrawingSpec(color=(245,66,230), thickness=2, circle_radius=2))

        # print(results)


        cv2.imshow("Mediapipe Feed", image)

        if cv2.waitKey(10) & 0xFF == ord('q'):
            break
cap.release()
cv2.destroyAllWindows()