import sys
import glob
import os
import ftplib


MAX_TRY_TO_REMOTE_COUNT = 30


g_exclude_dir = '.svn'
def IsExcludeDirectory(targetDirName):
    for at in g_exclude_dir:
        if at == targetDirName:
            return True

    return False




#get bundle version directory name
#only one directory in Patch/Android/
def GetBundleVersion():
    path = os.path.realpath(__file__)

    dirList = os.listdir('./')

    for dirName in dirList:
        if False == IsExcludeDirectory(dirName):
            if os.path.isdir(dirName):
                return dirName
        
    return ''

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




def GetLocalFileList(targetRoot):

    res = []
    
    for root, dirs, files in os.walk(targetRoot):

        for file in files:
            if root == targetRoot:
                res.append(file)

    return res

def GetLocalDirList(targetRoot):

    res = []
    
    for root, dirs, files in os.walk(targetRoot):

        for dir in dirs:
            if root == targetRoot:
                res.append(dir)

    return res



def RetrySendFile(srcFullpath, dstFullPath) :

    print("[RetrySendFile] --------------------------------------------------------- start")
    print(srcFullpath + " -> " + dstFullPath)

    isSuccess = False
    curTryCount = 0
    
    while curTryCount < MAX_TRY_TO_REMOTE_COUNT :
        
        fd = open(srcFullpath,'rb')                                # file to send
        if fd == 0:
            print("not found source file: " + srcFullpath)
            break

        curTryCount += 1
        print("try count: " + str(curTryCount))
        
        try:
            g_session.storbinary('STOR '+ dstFullPath, fd)      # send the file
        except:
            print("fail")
        else:
            isSuccess = True
            break
            

    print("Send Result: " + str(isSuccess) + ", try count: " + str(curTryCount) + "/" + str(MAX_TRY_TO_REMOTE_COUNT))

    if False == isSuccess :
        Force_Crash()
        
    print("[RetrySendFile] --------------------------------------------------------- end")
    
    return isSuccess


def RetryRemoveRemoteFile(dstFullPath) :

    print("[RetryRemoveRemoteFile] --------------------------------------------------------- start")
    print("dstFullPath : " + dstFullPath)

    isSuccess = False
    curTryCount = 0
    
    while curTryCount < MAX_TRY_TO_REMOTE_COUNT :

        curTryCount += 1
        print("try count: " + str(curTryCount))
        
        try:
            g_session.delete(dstFullPath)
        except:
            print("fail")
        else:
            isSuccess = True
            break
            

    print("Send Result: " + str(isSuccess) + ", try count: " + str(curTryCount) + "/" + str(MAX_TRY_TO_REMOTE_COUNT))

    if False == isSuccess :
        Force_Crash()
        
    print("[RetryRemoveRemoteFile] --------------------------------------------------------- end")
    
    return isSuccess


def RetryMakeRemoteDirectory(dstFullPath) :
    
    print("[RetryMakeRemoteDirectory] --------------------------------------------------------- start")
    print("dstFullPath : " + dstFullPath)

    isSuccess = False
    curTryCount = 0
    
    while curTryCount < MAX_TRY_TO_REMOTE_COUNT :

        curTryCount += 1
        print("try count: " + str(curTryCount))
        
        try:
            g_session.mkd(dstFullPath)
        except:
            print("fail")
        else:
            isSuccess = True
            break
            

    print("Send Result: " + str(isSuccess) + ", try count: " + str(curTryCount) + "/" + str(MAX_TRY_TO_REMOTE_COUNT))

    if False == isSuccess :
        Force_Crash()

    print("[RetryMakeRemoteDirectory] --------------------------------------------------------- end")
    
    return isSuccess


def RetryRemoveRemoteDirectory(dstFullPath) :

    print("[RetryRemoveRemoteDirectory]--------------------------------------------------------- start")
    print("dstFullPath : " + dstFullPath)

    isSuccess = False
    curTryCount = 0
    
    while curTryCount < MAX_TRY_TO_REMOTE_COUNT :

        curTryCount += 1
        print("try count: " + str(curTryCount))
        
        try:
            g_session.rmd(dstFullPath)
        except:
            print("fail")
        else:
            isSuccess = True
            break
            

    print("Send Result: " + str(isSuccess) + ", try count: " + str(curTryCount) + "/" + str(MAX_TRY_TO_REMOTE_COUNT))

    if False == isSuccess :
        Force_Crash()
    
    print("[RetryRemoveRemoteDirectory]--------------------------------------------------------- end")
    
    return isSuccess



def RetryRenameRemote(srcFullPath, dstFullPath) :
    
    print("[RetryRenameRemote] --------------------------------------------------------- start")
    print(srcFullPath + " -> " + dstFullPath)

    isSuccess = False
    curTryCount = 0
    
    while curTryCount < MAX_TRY_TO_REMOTE_COUNT :

        curTryCount += 1
        print("try count: " + str(curTryCount))
        
        try:
            g_session.rename(srcFullPath, dstFullPath)
        except:
            print("fail")
        else:
            isSuccess = True
            break
            

    print("Send Result: " + str(isSuccess) + ", try count: " + str(curTryCount) + "/" + str(MAX_TRY_TO_REMOTE_COUNT))

    if False == isSuccess :
        Force_Crash()
        
    print("[RetryRenameRemote] --------------------------------------------------------- end")
    
    return isSuccess


def RetryGetRemoteDirectoryList(dstFullPath):

    print("[RetryGetRemoteDirectoryList] --------------------------------------------------------- start")
    print("dstFullPath : " + dstFullPath)

    isSuccess = False
    curTryCount = 0

    remote_dir_list = -1
    
    while curTryCount < MAX_TRY_TO_REMOTE_COUNT :

        curTryCount += 1
        print("try count: " + str(curTryCount))
        
        try:
            #remote_dir_list = g_session.nlst(dstFullPath)
            ret = []
            g_session.dir(dstFullPath,ret.append)
            ret = [x.split()[-1] for x in ret if x.startswith("d")]
            remote_dir_list = ret
        except:
            print("fail")
        else:
            isSuccess = True
            break
            

    print("Send Result: " + str(isSuccess) + ", try count: " + str(curTryCount) + "/" + str(MAX_TRY_TO_REMOTE_COUNT))

    if False == isSuccess :
        Force_Crash()

    print("[RetryGetRemoteDirectoryList] --------------------------------------------------------- end")
    
    return remote_dir_list


def RetryGetRemoteFileList(dstFullPath):

    print("[RetryGetRemoteFileList]--------------------------------------------------------- start")
    print("dstFullPath : " + dstFullPath)

    isSuccess = False
    curTryCount = 0

    remote_dir_list = -1
    
    while curTryCount < MAX_TRY_TO_REMOTE_COUNT :

        curTryCount += 1
        print("try count: " + str(curTryCount))
        
        try:
            ret = []
            g_session.dir(dstFullPath,ret.append)
            ret = [x.split()[-1] for x in ret if x.startswith("-")]
            remote_dir_list = ret
        except:
            print("fail")
        else:
            isSuccess = True
            break
            

    print("Send Result: " + str(isSuccess) + ", try count: " + str(curTryCount) + "/" + str(MAX_TRY_TO_REMOTE_COUNT))

    if False == isSuccess :
        Force_Crash()
        
    print("[RetryGetRemoteFileList]--------------------------------------------------------- end")
    
    return remote_dir_list



def Make_Remote_Directory(dstRootDir, targetDir):

    print("--------------------")
    print("[Make_Remote_Directory] >>> " + dstRootDir + "/" + targetDir)
    print("dstRootDir : " + dstRootDir)
    print("targetDir : " + targetDir)
    print("--------------------")
    
    exist_target_dir = False

    remote_dir_list = RetryGetRemoteDirectoryList(dstRootDir)
    if remote_dir_list == -1 :
        return -1

    #print ""
    #print remote_dir_list
    #print "--------------------"

    for dir in remote_dir_list:
        
        #print "dir >> " + dir
        
        dirName = os.path.basename(dir)

        #print " loop dir name >>> " + dirName

        if dirName == targetDir:
            exist_target_dir = True
            break

    #print "--------------- exist_target_dir"
    #print exist_target_dir
    #print "--------------------------------"
    
    if False == exist_target_dir:
        make_dir_name = dstRootDir + "/" + targetDir
        if False == RetryMakeRemoteDirectory(make_dir_name):
            return -1
    


def Send_Directories(srcDir, dstRootDir, targetDir):

    if IsExcludeDirectory(targetDir):
        return

    Make_Remote_Directory(dstRootDir, targetDir)
    
    Send_Files(srcDir, dstRootDir, targetDir)
    
    #Send_Directories(srcDir, dstRootDir, targetDir)
    
    dirs = os.listdir(srcDir + '/')

    for dirName in dirs:

        #print "-=-=-=-=-=-=---== " + dirName
        #print "-=-=-=-=-=-=---== " + os.path.basename(dirName)
        
        fullpath = os.path.join(srcDir, dirName)
        
        if True == os.path.isdir(fullpath):
            
            nextTargetDir = os.path.basename(dirName)
            nextSrcDir = os.path.join(srcDir, nextTargetDir)
            nextRootDir = dstRootDir + "/" + targetDir #os.path.join(dstRootDir, targetDir)

            print("---------------------------------------------------------")
            print("[Send_Directories]")
            print("srcDir : " + srcDir)
            print("dstDir : " + dstRootDir + "/" + targetDir)
            print("targetDir : " + targetDir)
            print("nextSrcDir : " + nextSrcDir)
            print("nextRootDir : " + nextRootDir)
            print("nextTargetDir : " + nextTargetDir)
            print("---------------------------------------------------------")

            Send_Directories(nextSrcDir, nextRootDir, nextTargetDir)




# ex ) remote target dir = Release/Android/0_1
#       dstRootDir : Release/Android
#       targetDir : 0_1
def Send_Files(srcDir, dstRootDir, targetDir):
    #dirs = os.listdir(srcDir + '/')
    #dirs = dircache.listdir(srcDir + '/')
    files = GetLocalFileList(srcDir + '/')

    print("[Send_Files] files")
    print(files)
        
    for filename in files:

        #fullpath = os.path.abspath(filename)
        fullpath = os.path.join(srcDir, filename)

        #print "[Send_Files] fileName = " + filename
        #print "[Send_Files] fullPath = " + fullpath
        #print "[Send_Files] srcDir = " + srcDir
        
        upload_file_name = os.path.basename(filename)

        dstFullPath = dstRootDir + "/" + targetDir + "/" + upload_file_name
        print("Send >>> " + fullpath)
        print("  to     " + dstFullPath)
        print("file : " + upload_file_name)

        if False == RetrySendFile(fullpath, dstFullPath) :
            Force_Crash()
            return -1
            

def Remove_Remote_Directory(dstRootDir, targetDir):

    target_path = dstRootDir + "/" + targetDir

    print("---------------------------------------------------------")
    print("[Remove_Remote_Directory]")
    print("dstRootDir: " + dstRootDir)
    print("targetDir: " + targetDir)
    print("target_path: " + target_path)
    print("---------------------------------------------------------")


    # check target directory
    isExistTargetDir = False
    
    root_dir_list = RetryGetRemoteDirectoryList(dstRootDir)

    if root_dir_list == -1 :
        Force_Crash()
        return -1

    for tarDir in root_dir_list:
        
        if tarDir == targetDir:
            isExistTargetDir = True

    if False == isExistTargetDir:
        return



    file_list = RetryGetRemoteFileList(target_path)
    if file_list == -1 :
        Force_Crash()
        return -1
    
    if len(file_list) > 0:
        print(file_list)
        for file in file_list:

            full_path = target_path + "/" + file

            if False == RetryRemoveRemoteFile(full_path) :
                return -1
        
    dir_list = RetryGetRemoteDirectoryList(target_path)
    
    if dir_list == -1 :
        Force_Crash()
        return -1
    
    if len(dir_list) > 0:
        # print("[Remove_Remote_Directory] - remove target dir_list : " + dir_list)
        for dir in dir_list:
            if(dir == "." or dir == ".."):
                continue
            Remove_Remote_Directory(target_path, dir)


    if False == RetryRemoveRemoteDirectory(target_path) :
        return -1


    
def Rename_Remote_Directory(rootDirName, srcDirName, dstDirName):

    srcPath = rootDirName + "/" + srcDirName
    dstPath = rootDirName + "/" + dstDirName
    
    print("---------------------------------------------------------")
    print("[Rename_Remote_Directory]")
    print("srcPath: " + srcPath)
    print("dstPath: " + dstPath)
    print("---------------------------------------------------------")

    # check target directory
    isExistTargetDir = False
    root_dir_list = RetryGetRemoteDirectoryList(rootDirName)

    if root_dir_list == -1 :
        Force_Crash()
        return -1

    for tarDir in root_dir_list:
        
        if tarDir == srcDirName:
            isExistTargetDir = True

    if False == isExistTargetDir:
        return 0

    if False == RetryRenameRemote(srcPath, dstPath) :
        return -1

    return 0

       

def Change_Remote_Version_Name():

    change_dir_name = g_bundle_version_directory# + "_test"

    print("[Change_Remote_Version_Name] --------------------------------------------------------- start")
    print("---------------------------------------------------------")
    if -1 == Remove_Remote_Directory(".", change_dir_name + "_remove"):
        return -1
    print("---------------------------------------------------------")
    if -1 == Rename_Remote_Directory(".", change_dir_name, change_dir_name + "_remove"):
        return -1
    print("---------------------------------------------------------")
    if -1 == Rename_Remote_Directory(".", g_bundle_version_directory_temp, change_dir_name):
        return -1
    print("---------------------------------------------------------")
    if -1 == Remove_Remote_Directory(".", change_dir_name + "_remove"):
        return -1
    print("---------------------------------------------------------")
    print("[Change_Remote_Version_Name] --------------------------------------------------------- end")
    return 0



def Force_Crash():
    print("[Force_Crash]")
    raise



# main
REMOTE_URL = get_ftp_url()
REMOTE_ACCOUNT = get_ftp_account()
REMOTE_PASSWORD = get_ftp_password()

REMOTE_ROOT_NAME = get_root_name()
REMOTE_PRODUCT_NAME = get_product_name()
REMOTE_PLATFORM_NAME = get_ftp_platform() # Android or iOS
RELEASE_TYPE = get_ftp_release_type()
REMOTE_ROOT =  RELEASE_TYPE + '/' + REMOTE_PLATFORM_NAME


g_session = 0
g_bundle_version_directory = get_ftp_bundle_version()
g_bundle_version_directory_temp = g_bundle_version_directory + "_uploading_from_buildserver"

LOCAL_DIRECTORY = os.path.realpath(os.path.dirname(__file__)) + "/" + g_bundle_version_directory

print("============================================================")
print("local dir : " + LOCAL_DIRECTORY)
print("remote dir : " + REMOTE_ROOT + "/" + g_bundle_version_directory)
print("============================================================")

if len(g_bundle_version_directory) > 0:
    if len(REMOTE_PLATFORM_NAME) > 0:
        g_session = ftplib.FTP(REMOTE_URL, REMOTE_ACCOUNT, REMOTE_PASSWORD)

        if g_session:

            Make_Remote_Directory(".", REMOTE_ROOT_NAME)
            g_session.cwd(REMOTE_ROOT_NAME)
            
            Make_Remote_Directory(".", REMOTE_PRODUCT_NAME)
            g_session.cwd(REMOTE_PRODUCT_NAME)
            
            Make_Remote_Directory(".", RELEASE_TYPE)
            g_session.cwd(RELEASE_TYPE)
    
            #Upload_Override()
            Make_Remote_Directory(".", REMOTE_PLATFORM_NAME)
            g_session.cwd(REMOTE_PLATFORM_NAME)
            
            #[remove temp directory]
            try:
                Remove_Remote_Directory(".", g_bundle_version_directory_temp)
            except:
                print("[Error] failed remote temp directory. target directory name is : " + REMOTE_ROOT + "/" + g_bundle_version_directory_temp)
                g_session.quit()
                Force_Crash()

        
            #[uplaod all files]
            try:
                Send_Directories(LOCAL_DIRECTORY, ".", g_bundle_version_directory_temp)
            except:
                print("[Error] failed copy to remote.")
                g_session.quit()
                Force_Crash()
                

            #[change bundle directory name]
            try:
                Change_Remote_Version_Name()
            except:
                print("[Error] failed change rename temp directory of remote.")
                g_session.quit()
                Force_Crash()

                    
            g_session.quit()

            #[success send bundle data]
            print("upload success bundle data")

        else:
            print('[Error] could not make a connection to FTP');
            Force_Crash()
    else:
        print("[Error] invalid remote target device directory name. use argument 'Android' or 'iOS' ")
        Force_Crash()
else:
    print('[Error] not found bundle directory')
    Force_Crash()


"""
1. g_bundle_version_directory_temp(version_uploading_from_buildserver) 폴더 제거.
2. g_bundle_version_directory_temp 에 local file upload.
3. Change_Remote_Version_Name()
    3-1. version_remove 폴더 제거.
    3-2. 기존 version 폴더 version_remove 로 이름 변경.
    3-3. version_uploading_from_buildserver -> version 로 이름 변경.
    3-4. version_remove 폴더 제거.
    g_bundle_version_directory_temp -> g_bundle_version_directory(version) 로 변경
"""


