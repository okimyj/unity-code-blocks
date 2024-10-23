import os
from . import FUploader_sftp

MAX_RETRY_COUNT = 30


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


class Connector:
    uploader = None;

    def __init__(self):
        print('[Fuploader] init')

    def ConnectSftp(self, _url, _id, _key_path):
        print('[Fuploader] Create SFTP')
        self.uploader = FUploader_sftp.SFTPUploader(_url, _id, _key_path)

    def ConnectFtp(self):
        print('[Fuploader] Create FTP')
        raise

    def Close(self):
        print('================================================================')
        print('[close]')
        self.uploader.Close()
        print('----------------------------------------------------------------')

    def chdir(self, _target_dir):
        print('================================================================')
        print('[chdir] target_path : ' + _target_dir)
        self.uploader.chdir(_target_dir)
        print('----------------------------------------------------------------')

    def mkdir(self, _target_dir):
        print('================================================================')
        print('[mkdir] target_path : ' + _target_dir)
        if False == self.uploader.isExist(_target_dir):
            self.uploader.mkdir(_target_dir)
        print('----------------------------------------------------------------')
    def rmdir(self, _target_dir):
        print('================================================================')
        print('[rmdir] target_path : ' + _target_dir)

        dirList = self.uploader.getDirectoryList(_target_dir)

        if 0 != dirList:
            for atDir in dirList:
                self.rmdir(_target_dir + '/' + atDir)

            fileList = self.uploader.getFileList(_target_dir)

            if 0 != fileList:
                for atFile in fileList:
                    print('debug remove file : ' + _target_dir + '/' + atFile)
                    self.removeFile(_target_dir, atFile)

        if self.uploader.isExist(_target_dir):
            self.uploader.rmdir(_target_dir)

        print('----------------------------------------------------------------')

    def sendDirectory(self, _local_root, _local_dir, _target_root, _target_dir):
        localPath = _local_root + '/' + _local_dir
        targetPath = _target_root + '/' + _target_dir

        if len(_local_root) <= 1:
            localPath = _local_dir

        if len(_target_root) <= 1:
            targetPath = _target_dir

        print('================================================================')
        print('[sendDirectory] \nlocal_path = ' + localPath + '\ntarget_path = ' + targetPath)

        # make directory
        self.mkdir(targetPath)

        # upload files
        files = GetLocalFileList(localPath + '/')
        for atFile in files:
            fileName = os.path.basename(atFile)
            self.sendFile(localPath, fileName, targetPath, fileName)

        # send sub directory
        dirs = GetLocalDirList(localPath + '/')
        for atDir in dirs:
            self.sendDirectory(localPath, atDir, targetPath, atDir)

        print('----------------------------------------------------------------')

    def sendFile(self, _local_root, _local_file_name, _target_root, _target_file_name):
        localPath = _local_root + '/' + _local_file_name
        targetPath = _target_root + '/' + _target_file_name

        if len(_local_root) <= 1:
            localPath = _local_file_name

        if len(_target_root) <= 1:
            targetPath = _target_file_name

        print('================================================================')
        print('[sendFile] \nlocal_path = ' + localPath + '\ntarget_path = ' + targetPath)

        self.uploader.sendFile(localPath, targetPath)
        print('----------------------------------------------------------------')

    def removeFile(self, _target_path, _file_name):
        targetPath = _target_path + '/' + _file_name
        if len(_target_path) <= 1:
            targetPath = _file_name

        print('================================================================')
        print('[removeFile] file : ' + targetPath)
        if self.uploader.isExist(targetPath):
            self.uploader.removeFile(targetPath)
        print('----------------------------------------------------------------')

    def rename(self, _target_root, _src_name, _new_name):
        srcName = _target_root + '/' + _src_name
        newName = _target_root + '/' + _new_name

        if len(_target_root) <= 1:
            srcName = _src_name
            newName = _new_name

        print('================================================================')
        print('[rename] file : \n' + 'src : ' + srcName + '\n' + 'dst : ' + newName)

        if self.uploader.isExist(srcName):
            self.uploader.rename(srcName, newName)
        print('----------------------------------------------------------------')

    def pwd(self):
        self.uploader.pwd()
