import sys
import os
import ftplib


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

def get_build_type():

    if len(sys.argv) > 6:
        return sys.argv[6]

    return ""
def get_build_target():
    if(len(sys.argv)>7):
        return sys.argv[7]

def get_game_version():

    if len(sys.argv) > 8:
        return sys.argv[8]

    return ""

def get_origin_package_name():
    
    if len(sys.argv) > 9:
        return sys.argv[9]

    return ""

def get_deploy_package_name():
    
    if len(sys.argv) > 10:
        return sys.argv[10]

    return ""




def RetryMakeRemoteDirectory(dstFullPath) :
    
    print("---------------------------------------------------------")
    print("[RetryMakeRemoteDirectory]")
    print(dstFullPath)

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
        
    print("---------------------------------------------------------")
    
    return isSuccess

def RetryGetRemoteDirectoryList(dstFullPath):

    print("---------------------------------------------------------")
    print("[RetryGetRemoteDirectoryList]")
    print(g_session.pwd())
    print(dstFullPath)

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
        
    print("---------------------------------------------------------")
    
    return remote_dir_list

def Make_Remote_Directory(dstRootDir, targetDir):

    print("--------------------")
    print("[Make_Remote_Directory] >>> " + dstRootDir + "/" + targetDir)
    print("dstRootDir : " + dstRootDir)
    print("targetDir : " + targetDir)
    print(g_session.pwd())
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

        


def RetryGetRemoteFileList(dstRootPath):

    print("---------------------------------------------------------")
    print("[RetryGetRemoteDirectoryList]")
    print(dstRootPath)

    isSuccess = False
    curTryCount = 0

    remote_dir_list = -1
    
    while curTryCount < MAX_TRY_TO_REMOTE_COUNT :

        curTryCount += 1
        print("try count: " + str(curTryCount))
        
        try:
            ret = []
            g_session.dir(dstRootPath,ret.append)
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
        
    print("---------------------------------------------------------")
    
    return remote_dir_list


def RetryRemoveRemoteFile(dstRootPath, dstFileName) :

    print("---------------------------------------------------------")
    print("[RetryRemoveRemoteFile]")
    fullPath = dstRootPath + "/" + dstFileName
    print(fullPath)

    isSuccess = False
    curTryCount = 0

    while curTryCount < MAX_TRY_TO_REMOTE_COUNT :

        curTryCount += 1
        print("try count: " + str(curTryCount))
        
        try:
            g_session.delete(fullPath)
        except:
            print("fail")
        else:
            isSuccess = True
            break
            

    print("Send Result: " + str(isSuccess) + ", try count: " + str(curTryCount) + "/" + str(MAX_TRY_TO_REMOTE_COUNT))

    if False == isSuccess :
        Force_Crash()
        
    print("---------------------------------------------------------")
    
    return isSuccess


def RetrySendFile(srcFullpath, dstFullPath) :

    print("---------------------------------------------------------")
    print("[RetrySendFile]")
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
        
    print("---------------------------------------------------------")
    
    return isSuccess



def RetryRenameRemote(dstRootPath, srcFileName, dstFileName) :
    
    print("---------------------------------------------------------")
    print("[RetryRenameRemote]")
    print(dstRootPath + "/" + srcFileName + " -> " + dstRootPath + "/" + dstFileName)

    isSuccess = False
    curTryCount = 0
    
    while curTryCount < MAX_TRY_TO_REMOTE_COUNT :

        curTryCount += 1
        print("try count: " + str(curTryCount))
        
        try:
            g_session.rename(srcFileName, dstFileName)
        except:
            print("fail: " + str(sys.exc_info()[0]) + " " + str(sys.exc_info()[1]))
        else:
            isSuccess = True
            break
            

    print("Send Result: " + str(isSuccess) + ", try count: " + str(curTryCount) + "/" + str(MAX_TRY_TO_REMOTE_COUNT))

    if False == isSuccess :
        Force_Crash()
        
    print("---------------------------------------------------------")
    
    return isSuccess



def RenameRemoteFile(dstRootPath, srcFileName, dstFileName) :

    dirList = RetryGetRemoteFileList(dstRootPath)
    if dirList == -1 :
        return False

    for filename in dirList:

        if filename == dstFileName:
            print("exist try changed filename : " + dstRootPath + "/" + dstFileName)
            return False
            
        if filename == srcFileName:
            if False == RetryRenameRemote(dstRootPath, srcFileName, dstFileName) :
                return False

    return True
    

def RemoveRemoteFile(dstRootPath, dstFileName) :

    dirList = RetryGetRemoteFileList(dstRootPath)
    if dirList == -1 :
        return False

    for filename in dirList:
        
        if filename == dstFileName:
            
            if False == RetryRemoveRemoteFile(dstRootPath, dstFileName) :
                return False

    return True


def Force_Crash():
    print("[Force_Crash]")
    raise



g_session = 0
package_file_name = get_origin_package_name()
package_deploy_file_name = get_deploy_package_name()
uploadTemp_package_file_name = package_file_name + "_temp"
removeTemp_package_file_name = package_file_name + "_remove"

MAX_TRY_TO_REMOTE_COUNT = 30

LOCAL_ROOT = os.path.dirname( os.path.abspath( __file__ ) )

if len(package_file_name) > 0:

    print("upload package: " + package_file_name)
    

    PackageFullPath = LOCAL_ROOT + "/" + package_file_name
    print("PackageFullPath : " + PackageFullPath)

    if os.path.exists(PackageFullPath):
    
        g_session = ftplib.FTP(get_ftp_url(),get_ftp_account(),get_ftp_password())

        root_name = get_root_name()
        product_name = get_product_name()
        build_type_name = get_build_type()
        build_target_name = get_build_target()
        game_version_name = get_game_version()
        
        Make_Remote_Directory(".", root_name)
        g_session.cwd(root_name)

        Make_Remote_Directory(".", product_name)
        g_session.cwd(product_name)

        Make_Remote_Directory(".", build_type_name)
        g_session.cwd(build_type_name)

        Make_Remote_Directory(".", build_target_name)
        g_session.cwd(build_target_name)

        if (len(game_version_name) > 0 and (game_version_name != "None" and game_version_name != "none")):
            Make_Remote_Directory(".", game_version_name);
            g_session.cwd(game_version_name)

        print("")
        print("")
        print("/////////////////////////////////////////////////////////")
        print("---------------------------------------------------------")
        print("[Upload Root]")
        print(g_session.pwd())
        print("---------------------------------------------------------")
        print("/////////////////////////////////////////////////////////")
        print("")
        print("")

        # 이전에 삭제 실패한 임시 업로드파일 제거
        print("[step 1] remove : " + uploadTemp_package_file_name)
        if False == RemoveRemoteFile(".", uploadTemp_package_file_name):
            g_session.quit()
            Force_Crash()
            #return -1

        # 이전에 삭제 실패한 임시 삭제파일 제거
        print("[step 2] remove : " + removeTemp_package_file_name)
        if False == RemoveRemoteFile(".", removeTemp_package_file_name):
            g_session.quit()
            Force_Crash()
            #return -1


        # 패키지 임시 파일명으로 업로드
        print("[step 3] upload : " + package_file_name + " -> " + uploadTemp_package_file_name)
        if False == RetrySendFile(PackageFullPath, uploadTemp_package_file_name) :
            g_session.quit()
            Force_Crash()
            #return -1


        # 이전에 배포되어 있던 패키지 파일 제거대상으로 이름변경
        print("[step 4] rename : " + package_file_name + " -> " + removeTemp_package_file_name)
        if False == RenameRemoteFile(".", package_deploy_file_name, removeTemp_package_file_name):
            g_session.quit()
            Force_Crash()
            #return -1

        # 새로 올린업로드 파일 배포할 패키지 이름으로 변경
        print("[step 5] rename : " + uploadTemp_package_file_name + " -> " + package_deploy_file_name)
        if False == RenameRemoteFile(".", uploadTemp_package_file_name, package_deploy_file_name):
            g_session.quit()
            Force_Crash()
            #return -1

        # 이전에 배포되어 있던 패키지 파일 제거
        print("[step 6] remove : " + removeTemp_package_file_name)
        if False == RemoveRemoteFile(".", removeTemp_package_file_name):
            g_session.quit()
            Force_Crash()
            #return -1

        g_session.quit()

        print("upload success package data")
        
    else:
        print("[Error] not found file: " + package_file_name)
        raise NotImplementedError
else:
    print("[Error] invalid package name. use argument 'packagename.extension'")
    raise NotImplementedError
