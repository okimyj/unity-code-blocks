import sys
import os
from FUploaderClass import FUploader

INTERNAL_TEST = False

def GetArg(_index, _internal_test_value):
    if (INTERNAL_TEST):
        return _internal_test_value

    return sys.argv[_index]

IS_SFTP_PROTOCOL = GetArg(1, 'SFTP')
REMOTE_URL = GetArg(2, '133.186.134.79')
REMOTE_ID = GetArg(3, 'root')
REMOTE_PW_OR_KEY_NAME = GetArg(4, 'marble-keypairs.pem')
UPLOAD_ROOT = GetArg(5, 'Bundle')
PRODUCT_NAME = GetArg(6, 'Marble')
RELEASE_TYPE = GetArg(7, 'testVersion')
PLATFORM_NAME = GetArg(8, 'Android')
GAME_VERSION = GetArg(9, '1_0_0')

IS_SFTP = True
if IS_SFTP_PROTOCOL != 'SFTP' and IS_SFTP_PROTOCOL != 'sftp':
    IS_SFTP = False


UPLOAD_LIST_FILE = 'upload_list__' + GAME_VERSION + '.txt'
UPLOAD_LIST_FILE_FULL_PATH = os.path.realpath(os.path.dirname(__file__)) + "/" + UPLOAD_LIST_FILE
PRIVATE_KEY_FULL_PATH = os.path.realpath(os.path.dirname(__file__)) + "/" + REMOTE_PW_OR_KEY_NAME

def GetUploadList():
    file_list = []
    try:
        file_data = open(UPLOAD_LIST_FILE_FULL_PATH)
        try:
            for read_line in file_data:
                file_list.append(read_line)
        finally:
            file_data.close()
    except:
        print 'not found ' + UPLOAD_LIST_FILE
        raise
    return file_list;


def SaveRemainUploadList():
    save_file = open(UPLOAD_LIST_FILE_FULL_PATH, 'w')

    for save_line in upload_list:
        save_file.write(save_line)

    save_file.close()

try:
    conn = FUploader.Connector();

    # open session
    if IS_SFTP:
        conn.ConnectSftp(REMOTE_URL, REMOTE_ID, PRIVATE_KEY_FULL_PATH)
    else:
        conn.ConnectFtp(REMOTE_URL, REMOTE_ID, REMOTE_PW_OR_KEY_NAME)
except:
    raise

try:
    versionFullPath = os.path.realpath(os.path.dirname(__file__)) + "/" + GAME_VERSION

    # make and change directory
    conn.chdir(UPLOAD_ROOT)
    conn.chdir(PRODUCT_NAME)
    conn.chdir(RELEASE_TYPE)
    conn.chdir(PLATFORM_NAME)
    conn.chdir(GAME_VERSION)

    upload_list = GetUploadList()
    total_count = len(upload_list)

    print os.path.realpath(os.path.dirname(__file__))

    while len(upload_list) > 0:
        (_hash, _bundle) = upload_list[0].split(':', 1)
        _bundle = _bundle.strip()

        curr_index = (total_count - len(upload_list)) + 1

        conn.removeFile(_hash, _bundle)
        conn.mkdir(_hash)
        localPath = versionFullPath + '/' + _hash
        conn.sendFile(localPath, _bundle, _hash, _bundle)

        del upload_list[0]
        SaveRemainUploadList()

    # close session
    conn.Close()
except:
    conn.Close()
    raise
