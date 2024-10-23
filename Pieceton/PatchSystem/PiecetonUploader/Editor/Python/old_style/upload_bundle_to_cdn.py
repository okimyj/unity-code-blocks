import sys
import glob
import os
import ftplib

#-----------------------------------------------------
# argument functions

def get_ftp_url():

    if len(sys.argv) > 1:
        return sys.argv[1]

    return ""

def get_ftp_account():
    if len(sys.argv) > 2:
        return sys.argv[2]

    return ""

def get_ftp_password():
    if len(sys.argv) > 3:
        return sys.argv[3]

    return ""

def get_root_name():
    if len(sys.argv) > 4:
        return sys.argv[4]

    return ""

def get_product_name():
    if len(sys.argv) > 5:
        return sys.argv[5]

    return ""

def get_ftp_release_type():
    if len(sys.argv) > 6:
        return sys.argv[6]

    return ""

def get_ftp_platform():
    if len(sys.argv) > 7:
        return sys.argv[7]

    return ""

def get_ftp_bundle_version():
    if len(sys.argv) > 8:
        return sys.argv[8]

    return ""


# retry functions

def reTry_mkd(directoryName):

    print("---------------------------------------------------------")
    print("[reTry_mkd]")
    print(directoryName)
    
    isSuccess = False
    curTryCount = 0
    
    while curTryCount < RETRY_MAX_COUNT :

        curTryCount += 1
        print("try count: " + str(curTryCount))
       
        try:
            g_session.mkd(directoryName)
        except:
            if curTryCount >= RETRY_MAX_COUNT:
                Force_Crash("mkd: " + directoryName)
        else:
            isSuccess = True
            break;

    print("mkd: try count: " + str(curTryCount) + "/" + str(RETRY_MAX_COUNT))

    if False == isSuccess :
        Force_Crash()

    print("---------------------------------------------------------")

def reTry_delete(deleteName):

    print("---------------------------------------------------------")
    print("[reTry_delete]")
    print(deleteName)
    
    isSuccess = False
    curTryCount = 0
    
    while curTryCount < RETRY_MAX_COUNT :

        curTryCount += 1
        print "try count: " + str(curTryCount)
       
        try:
            g_session.delete(deleteName)
        except:
            if curTryCount >= RETRY_MAX_COUNT:
                Force_Crash("delete: " + deleteName)
        else:
            isSuccess = True
            break;

    print("delete: try count: " + str(curTryCount) + "/" + str(RETRY_MAX_COUNT))

    if False == isSuccess :
        Force_Crash()

    print("---------------------------------------------------------")


def reTry_storbinary(file_data, send_file_name):

    print("---------------------------------------------------------")
    print("[reTry_storbinary]")
    print(send_file_name)
    
    isSuccess = False
    curTryCount = 0
    
    while curTryCount < RETRY_MAX_COUNT :

        curTryCount += 1
        print("try count: " + str(curTryCount))
       
        try:
            g_session.storbinary('STOR '+ send_file_name, file_data)
        except:
            if curTryCount >= RETRY_MAX_COUNT:
                Force_Crash("storbinary: " + send_file_name)
        else:
            isSuccess = True
            break

    print("storbinary: try count: " + str(curTryCount) + "/" + str(RETRY_MAX_COUNT))

    if False == isSuccess :
        Force_Crash()

    print("---------------------------------------------------------")


def reTry_dir(rootPath):

    print("---------------------------------------------------------")
    print("[reTry_dir]")
    print(rootPath)
    
    isSuccess = False
    remote_dir_list = []
    curTryCount = 0
    
    while curTryCount < RETRY_MAX_COUNT :

        curTryCount += 1
        print("try count: " + str(curTryCount))
       
        try:
            ret = []
            g_session.dir(rootPath, ret.append)
            ret = [x.split()[-1] for x in ret if x.startswith("d")]
            remote_dir_list = ret
        except:
            if curTryCount >= RETRY_MAX_COUNT:
                Force_Crash("dir: " + rootPath)
        else:
            isSuccess = True
            break;

    print("dir: try count: " + str(curTryCount) + "/" + str(RETRY_MAX_COUNT))

    if False == isSuccess :
        Force_Crash()

    print("---------------------------------------------------------")

    return remote_dir_list


def reTry_dir_files(rootPath):

    print("---------------------------------------------------------")
    print("[reTry_dir_files]")
    print(rootPath)
    
    isSuccess = False
    remote_dir_list = []
    curTryCount = 0
    
    while curTryCount < RETRY_MAX_COUNT :

        curTryCount += 1
        print("try count: " + str(curTryCount))
       
        try:
            ret = []
            g_session.dir(rootPath, ret.append)
            ret = [x.split()[-1] for x in ret if x.startswith("-")]
            remote_dir_list = ret
        except:
            if curTryCount >= RETRY_MAX_COUNT:
                Force_Crash("dir: " + rootPath)
        else:
            isSuccess = True
            break

    print("dir files: try count: " + str(curTryCount) + "/" + str(RETRY_MAX_COUNT))

    if False == isSuccess :
        Force_Crash()

    print("---------------------------------------------------------")

    return remote_dir_list


def reTry_pwd():

    print("---------------------------------------------------------")
    print("[reTry_pwd]")
    
    isSuccess = False
    remote_pwd = ""
    curTryCount = 0
    
    while curTryCount < RETRY_MAX_COUNT :

        curTryCount += 1
        print("try count: " + str(curTryCount))
       
        try:
            remote_pwd = g_session.pwd()
        except:
            if curTryCount >= RETRY_MAX_COUNT:
                Force_Crash("pwd")
        else:
            isSuccess = True
            break

    print("pwd: try count: " + str(curTryCount) + "/" + str(RETRY_MAX_COUNT))

    if False == isSuccess :
        Force_Crash()

    print("---------------------------------------------------------")
    
    return remote_pwd


def reTry_cwd(directoryName):

    print("---------------------------------------------------------")
    print("[reTry_pwd]")
    print(directoryName)
    
    isSuccess = False
    curTryCount = 0
    
    while curTryCount < RETRY_MAX_COUNT :

        curTryCount += 1
        print("try count: " + str(curTryCount))
       
        try:
            g_session.cwd(directoryName)
        except:
            if curTryCount >= RETRY_MAX_COUNT:
                Force_Crash("cwd: " + directoryName)
        else:
            isSuccess = True
            break;

    print("cwd: try count: " + str(curTryCount) + "/" + str(RETRY_MAX_COUNT))

    if False == isSuccess :
        Force_Crash()

    print("---------------------------------------------------------")


#-----------------------------------------------------
#
def IsExistRemoteHashFolder(hashName):
    remote_dir_list = reTry_dir(reTry_pwd())

    for dir in remote_dir_list:
        dirName = os.path.basename(dir)

        if dirName == hashName:
            return True

    return False

    

def SendBundle(srcRoot, hashName, bundleName, curr_index):

    print("[SendBundle " + curr_index + "/" + str(total_count) + "] " + reTry_pwd() + "/" + hashName + "/" + bundleName)

    srcFullPath = srcRoot + "/" + hashName

    if False == os.path.exists(srcFullPath):
        Force_Crash("not found local directory:\n " + srcFullPath)
    
    if False == IsExistRemoteHashFolder(hashName):
        reTry_mkd(hashName)

    file_list = reTry_dir_files(hashName)
    
    for remoteFile in file_list:
        removeFileFullPath = hashName + "/" + remoteFile
        reTry_delete(removeFileFullPath)

    uploadFile = srcFullPath + "/" + bundleName
    dstFilePath = hashName + "/" + bundleName

    try:
        fd = open(uploadFile,'rb')                                # file to send
    except:
        Force_Crash("not found local bundle file:\n" + uploadFile)
    else:
        reTry_storbinary(fd, dstFilePath)
        fd.close()
      

def SafeChangeRemoteDirectory(targetDir):
    #print "---------------------------------------------------------"
    #print "[SafeChangeRemoteDirectory(] "
    #print "pwd: " + g_session.pwd()
    #print "targetDir: " + targetDir
    remote_dir_list = reTry_dir(reTry_pwd())

    #print remote_dir_list

    exist_target_dir = False

    for dir in remote_dir_list:
        dirName = os.path.basename(dir)
        
        if dirName == targetDir:
            exist_target_dir = True
            break
    
    if False == exist_target_dir:
        reTry_mkd(targetDir)

    reTry_cwd(targetDir)


def GetUploadList():
    file_list = []

    try:
        #file_data = open(UPLOAD_LIST_FILE)
        file_data = open(UPLOAD_LIST_FILE_FULL_PATH)

        try:
            for read_line in file_data:
                file_list.append(read_line)
                
        finally:
            file_data.close()
            
    except:
        print('not found ' + UPLOAD_LIST_FILE)
        
        
    return file_list;


def SaveRemainUploadList():
    
    #save_file = open(UPLOAD_LIST_FILE, 'w')
    save_file = open(UPLOAD_LIST_FILE_FULL_PATH, 'w')

    #print(upload_list, file=save_file)

    for save_line in upload_list:
        save_file.write(save_line)
    
    save_file.close()
    

    
def Force_Crash(desc):
    print("-------------------- [Force_Crash] --------------------")
    print(desc)
    print("-------------------------------------------------------")
    raise




#-----------------------------------------------------
#
REMOTE_CDN_ROOT = get_root_name()
REMOTE_PRODUCT = get_product_name()
REMOTE_URL = get_ftp_url()
REMOTE_ACCOUNT = get_ftp_account()
REMOTE_PASSWORD = get_ftp_password()

RELEASE_NAME = get_ftp_release_type()
PLATFORM_NAME = get_ftp_platform() # Android or iOS
BUNDLE_VERSION = get_ftp_bundle_version() # 0_1

UPLOAD_LIST_FILE = 'upload_list__' + BUNDLE_VERSION + '.txt'

REMOTE_ROOT = RELEASE_NAME + '/' + PLATFORM_NAME

LOCAL_ROOT = os.path.realpath(os.path.dirname(__file__)) + "/" + BUNDLE_VERSION

UPLOAD_LIST_FILE_FULL_PATH = os.path.realpath(os.path.dirname(__file__)) + "/" + UPLOAD_LIST_FILE


RETRY_MAX_COUNT = 30

#-----------------------------------------------------



print("============================================================")
print("local dir : " + LOCAL_ROOT)
print("remote dir : " + REMOTE_ROOT + "/" + BUNDLE_VERSION)
print("------------------------------------------------------------")

upload_list = GetUploadList()
total_count = len(upload_list)
if total_count > 0:

    print("url: " + REMOTE_URL)
    print("id: " + REMOTE_ACCOUNT)
    print("ps: " + REMOTE_PASSWORD)
    
    g_session = ftplib.FTP(REMOTE_URL, REMOTE_ACCOUNT, REMOTE_PASSWORD)

    if g_session:

        # create remote target directory.
        SafeChangeRemoteDirectory(REMOTE_CDN_ROOT)
        SafeChangeRemoteDirectory(REMOTE_PRODUCT)
        SafeChangeRemoteDirectory(RELEASE_NAME)
        SafeChangeRemoteDirectory(PLATFORM_NAME)
        SafeChangeRemoteDirectory(BUNDLE_VERSION)

        while len(upload_list) > 0:

            if len(upload_list[0]) > 10:
            
                (_hash, _bundle) = upload_list[0].split(':', 1)
                _bundle = _bundle.strip()

                curr_index = (total_count - len(upload_list)) + 1
            
                SendBundle(LOCAL_ROOT, _hash, _bundle, str(curr_index))

            print("remove : '" + upload_list[0] + "'")
            del upload_list[0]

            SaveRemainUploadList()
    else:
        Force_Crash('could not make a connection to FTP')
else:
    print('[Success] empty upload list')
    
print("============================================================")
