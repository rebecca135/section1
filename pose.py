#WORKED ON COMPUTER 1 BUT NOT ANOTHER FOR SOME REASON. I WILL FIGURE THAT OUT LATER

import cv2
import mediapipe as mp
import socket
import json

# UDP receiver ip & port
# UDP_IP = "10.137.19.131"
UDP_IP = "127.0.0.1"
UDP_PORT = 10001

data = {
    "hw" : {
        "yR" : 0,
        "st" : 0
    }
}

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
    yRel = "{:.2f}".format(yRel)
    yRel = float(yRel)
    return yRel

sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM) # UDP
yRel = 0

cap = cv2.VideoCapture(0)
with mp_pose.Pose(min_detection_confidence=0.5, min_tracking_confidence=0.5) as pose:

    while cap.isOpened():
        ret, frame = cap.read()
        # Recolor image to RGB
        image = cv2.cvtColor(frame, cv2.COLOR_BGR2RGB)
        image.flags.writeable = False

        # Make detectionq
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
            print(yRel)
        # Send UDP message to target
            #data["highest_wrist"]["yRel"] = yRel
            #MESSAGE = bytes(json.dumps(data), "ascii")
            #print(MESSAGE)
            sock.sendto(str(yRel).encode('UTF-8'), (UDP_IP, UDP_PORT))    
        except:
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