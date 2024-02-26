#2024.2.26 python版多线程ping

import subprocess
import ipaddress
import threading

lock = threading.Lock()

def ping_ip(ip, output_file):
    result = subprocess.Popen(['ping', '-n', '2', ip], stdin=subprocess.PIPE,stdout=subprocess.PIPE,stderr=subprocess.PIPE,shell=True)
    out,err=result.communicate()
    print(ip + "----" + str(result.returncode))
    with lock:
        with open(output_file, 'a') as f:
            if result.returncode == 0:
                f.write(f"{ip} is reachable\n")
            else:
                f.write(f"{ip} is unreachable\n")

def ping_network(network, output_file):
    for i in range(1, 256):
        ip = f"{network}.{i}"
        thread = threading.Thread(target=ping_ip, args=(ip, output_file))
        thread.start()

def main():
    networks = ["10.64.136", "10.64.137", "10.64.138"]
    output_file = "result.txt"

    threads = []
    for network in networks:
        thread = threading.Thread(target=ping_network, args=(network, output_file))
        threads.append(thread)
        thread.start()

    for thread in threads:
        thread.join()

if __name__ == "__main__":
    main()
