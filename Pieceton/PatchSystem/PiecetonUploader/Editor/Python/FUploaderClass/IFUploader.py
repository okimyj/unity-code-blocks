class BaseFUploader:
    def __init__(self):
        print('[BaseFUploader] init')

    def Close(self):
        raise '[BaseFUploader] Close. no override'

    def chdir(self, _target_dir):
        raise '[BaseFUploader] chdir. no override'

    def mkdir(self):
        raise '[BaseFUploader] mkdir. no override'

    def rmdir(self):
        raise '[BaseFUploader] rmdir. no override'

    def isExist(self):
        raise '[BaseFUploader] isExist. no override'

    def getDirectoryList(self):
        raise '[BaseFUploader] getDirectoryList. no override'

    def getFileList(self):
        raise '[BaseFUploader] getFileList. no override'

    def sendFile(self):
        raise '[BaseFUploader] sendFile. no override'

    def removeFile(self):
        raise '[BaseFUploader] removeFile. no override'

    def rename(self):
        raise '[BaseFUploader] rename. no override'

    def pwd(self):
        raise '[BaseFUploader] pwd. no override'
