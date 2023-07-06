from PIL import Image
import logging
import socket

def send_string(message, ip, port):
    sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    try:
        sock.connect((ip, port))
        sock.sendall(message.encode())
    finally:
        sock.close()

# =========== EDIT THIS ===========

ip_address = '127.0.0.1' #localhost
port_number = 6969 #defaut port
pixel = ("█") #recommended
height = "<line-height=90%>" #recommended
imgpath = ("1.png") #image path

# =========== EDIT THIS ===========

logging.basicConfig(format='[%(asctime)s] %(message)s')
logging.warning("PIL started opening image at path -> " + imgpath)
img = Image.open(imgpath)
logging.warning(f"PIL opening thread probably stopped")
logging.warning(f"Format: {img.format}")
h, w = img.size
logging.warning(f"Dimensions: width={h} height={w}")
logging.warning(f"Color Mode: {img.mode} ")
pixels = h * w
logging.warning(f"Overall pixels: {pixels} ")
logging.warning(f"Making sure it's RGB")
rgb_img = img.convert('RGB')

def rgb_to_hex(rgb):
    return '%02x%02x%02x' % rgb

def compresscode(unccode):
    compresswhite = unccode.replace("#ffffff", "#fff")
    compressblack = compresswhite.replace("#000000", "#000")
    compressred = compressblack.replace("#ff0000", "red")
    compressyellow = compressred.replace("#ffff00", "yellow")
    compressgreen = compressyellow.replace("#00ff00", "green")
    compresscyan = compressgreen.replace("#00ffff", "cyan")
    compressblue = compresscyan.replace("#0000ff", "blue")
    return compressblue

tw = 0
th = 0
columns = [height]
code = []

logging.warning(f"Converting...")

while tw < w:
    column = []
    th = 0
    while th <= h:
        if th == h:
            break
        r, g, b = rgb_img.getpixel((th, tw))
        rgb = (r, g, b)
        hexvalue = ("#" + rgb_to_hex(rgb))
        formula = (f"<color={hexvalue}>" + pixel)
        column.append(formula)
        th += 1
    finished_column = "".join(column)
    columns.append(f"{finished_column}")
    tw += 1  

code = "\n".join(columns)

logging.warning(f"Uncompressed code length -> {len(code)}")
logging.warning("Saving uncompressed HTML code...")

with open('output_uncompressed.txt', 'w', encoding="utf-8") as f:
    f.write(code)

logging.warning("Compressing...")
compressedcode = compresscode(code)

logging.warning(f"Compressed code length -> {len(compressedcode)}")
logging.warning("Saving compressed HTML code...")
cratio = len(code) / len(compressedcode) * 100
logging.warning(f"Compression ratio -> {cratio}%")

with open('output_compressed.txt', 'w', encoding="utf-8") as f:
    f.write(compressedcode)

logging.warning("Sending to CommandSocket...")
message_to_send = (f"/icomtxt {compressedcode}")
send_string(message_to_send, ip_address, port_number)