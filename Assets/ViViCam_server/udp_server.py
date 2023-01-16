import triad_openvr
import time
import sys
import struct
import socket
import ctypes
winmm = ctypes.WinDLL('winmm')
winmm.timeBeginPeriod(1)

remove_elements_flag = False
start = time.time()

# Removes inactive tracking devices
def cleanup_tracked_list():
    tracked_error_list = []

    for device_string in v.devices.keys():
        if (v.devices[device_string].get_pose_quaternion() is None):
            print("[Warning]: Tracking device \"" + device_string + "\" is inactive! Removing from list.")
            tracked_error_list.append(device_string)

    for device in tracked_error_list:
        v.devices.pop(device)

    remove_elements_flag = False

##### INIT #####
sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
server_address = ('127.0.0.1', 8051)

v = triad_openvr.triad_openvr()
v.print_discovered_objects()

if len(sys.argv) == 1:
    interval = 1/250 - 0.001 # 250Hz / 4ms - 1ms, since execution takes ~1ms and time.sleep can only sleep for "whole" ms.
                             # (Will need to calibrate depending on the computer used for tracking)
                             # Advantage is less noisy sleep delays
elif len(sys.argv) == 2:
   interval = 1/float(sys.argv[1]) - 0.001
else:
    print("Invalid number of arguments")
    interval = False
    

cleanup_tracked_list()


# floats = [0.1253, 0.157446, 0.968451, 0.87453618, 0.8968, 0.84865465, 0.843872]

##### MAIN LOOP #####
if interval:
    while(True):
        start = time.time()

        data = bytes(struct.pack("i", len(v.devices)))

        for device_string in v.devices.keys():
            data += bytes(device_string + ":", "ascii")

            if (v.devices[device_string].get_pose_quaternion() is None):
                remove_elements_flag = True
            else:
                for value in v.devices[device_string].get_pose_quaternion():
                    data += bytearray(struct.pack("f", value))

        ### Fake data sender
        # for device_string in ["hmd_1", "controller_1"]:
        #     data = bytes(device_string + ":", "ascii")
        #     for value in floats:
        #         data += bytearray(struct.pack("f", value))
        
        # for debug: print sent data to terminal
        # print(data, end="")
        # print("\r")

        if remove_elements_flag:
            cleanup_tracked_list()

        sock.sendto(data, server_address)
        # sleep_time = interval - (time.time() - start)
        # time.sleep(sleep_time if sleep_time > 0 else 0)
        time.sleep(interval)