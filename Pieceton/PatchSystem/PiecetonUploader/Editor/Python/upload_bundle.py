import os
import sys
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

PRIVATE_KEY_FULL_PATH = os.path.realpath(os.path.dirname(__file__)) + "/" + REMOTE_PW_OR_KEY_NAME

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
    # make and change directory
    conn.chdir(UPLOAD_ROOT)
    conn.chdir(PRODUCT_NAME)
    conn.chdir(RELEASE_TYPE)
    conn.chdir(PLATFORM_NAME)

    uploadDirName = GAME_VERSION + '_upload'
    removeDirName = GAME_VERSION + '_remove'

    # remove unable files
    conn.rmdir(uploadDirName)
    conn.rmdir(removeDirName)

    # upload version directory
    localPath = os.path.realpath(os.path.dirname(__file__))
    conn.sendDirectory(localPath, GAME_VERSION, '.', uploadDirName)

    # change deploy directory name
    conn.rename('.', GAME_VERSION, removeDirName)
    conn.rename('.', uploadDirName, GAME_VERSION)

    # remove old directory
    conn.rmdir(removeDirName)

    # close session
    conn.Close()
except:
    conn.Close()
    raise