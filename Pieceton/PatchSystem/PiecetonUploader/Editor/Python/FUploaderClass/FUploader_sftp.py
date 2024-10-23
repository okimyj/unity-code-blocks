import os
import errno
import paramiko
from . import IFUploader

class SFTPUploader(IFUploader.BaseFUploader):
    m_ssh = 0
    m_sftp = 0

    def __init__(self, _url, _id, _key_path):
        _url = "192.168.0.100"
        print('----------------------------------------------------------------')
        print('[Connector] init: ' + 'url = ' + _url + ' id = ' + _id + ' pw = ' + _key_path)
        _, ext = os.path.splitext(_key_path)
        
        usePem = ext == '.pem'
        
        if usePem :
            key = paramiko.RSAKey.from_private_key_file(_key_path)
            self.m_ssh = paramiko.SSHClient()  # will create the object
            self.m_ssh.set_missing_host_key_policy(paramiko.AutoAddPolicy())  # no known_hosts error
            self.m_ssh.connect(_url, username=_id, pkey=key)  # no passwd needed   
            transport = paramiko.Transport((_url, 22))
            transport.connect(username=_id, pkey=key)
        else:
            key = os.path.basename(_key_path)
            transport = paramiko.Transport((_url, 22))
            transport.connect(username=_id, password=key)

        print('[Connector] final info : ' + 'url = ' + _url + ' id = ' + _id + ' pw = ' + key + ' usePem = ' + str(usePem))

        
        self.m_sftp = paramiko.SFTPClient.from_transport(transport)

        

    def Close(self):
        self.m_sftp.close()
        if self.m_ssh != 0 :
            self.m_ssh.close()

    def chdir(self, _target_dir):
        try:
            self.m_sftp.chdir(_target_dir)
        except:
            self.m_sftp.mkdir(_target_dir)
            self.m_sftp.chdir(_target_dir)

    def mkdir(self, _target_dir):
        self.m_sftp.mkdir(_target_dir)

    def rmdir(self, _target_dir):
        self.m_sftp.rmdir(_target_dir)

    def getDirectoryList(self, _target_root):
        resList = []

        try:
            for i in self.m_sftp.listdir(_target_root):
                atPath = _target_root + '/' + i
                lstatout = str(self.m_sftp.lstat(atPath)).split()[0]

                if 'd' in lstatout:
                    resList.append(i)

        except IOError:
            return 0

        return resList

    def getFileList(self, _target_root):
        resList = []

        try:
            for i in self.m_sftp.listdir(_target_root):
                atPath = _target_root + '/' + i
                lstatout = str(self.m_sftp.lstat(atPath)).split()[0]

                if 'd' not in lstatout:
                    resList.append(i)

        except IOError:
            return 0

        return resList

    def isExist(self, target_path):
        try:
            self.m_sftp.stat(target_path)
        except IOError as e:
            # 'No such file' in str(e):
            if e.errno == errno.ENOENT:
                return False
        else:
            return True

    def sendFile(self, _local_path, _target_path):
        self.m_sftp.put(localpath=_local_path, remotepath=_target_path)

    def removeFile(self, _target_path):
        self.m_sftp.remove(_target_path)

    def rename(self, _src_name, _new_name):
        self.m_sftp.rename(_src_name, _new_name)

    def pwd(self):
        print(self.m_sftp.getcwd())

