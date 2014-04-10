# //////////////////////////////////////////////////////////////////////
# //
# // Copyright (c) 2012 Audiokinetic Inc. / All Rights Reserved
# //
# //////////////////////////////////////////////////////////////////////
import sys, os, os.path, argparse, subprocess, logging, shutil
from os import pardir, makedirs, removedirs
from os.path import dirname, abspath, join, exists
import BuildUtil

class BuildInitializer(object):
    '''For a specified platform. Create deployment folders. Cleanup old generated files.'''

    def __init__(self, pathMan):
        self.pathMan = pathMan
        self.logger = BuildUtil.CreateLogger(pathMan.Paths['Log'], __file__, self.__class__.__name__)

    def Initialize(self):
        self._RemoveDeployment()
        self._MakeDeploymentDirs()

    def _MakeDeploymentDirs(self):
        self._SafeMakeDirs(self.pathMan.Paths['Deploy_API_Generated_Common'])
        self._SafeMakeDirs(self.pathMan.Paths['Deploy_API_Generated_Platform'])
        self._SafeMakeDirs(self.pathMan.Paths['Deploy_Plugins'])

    def _SafeMakeDirs(self, p):
        if not os.path.isdir(p):
            try:
                makedirs(p)
            except Exception as e:
                self.logger.info('Failed to create folder {} with error: {}. Ignored.'.format(p, e))

    def _RemoveDeployment(self):
        self._RemoveGeneratedAPIs()

    def _RemoveGeneratedAPIs(self):
        self._SafeRemoveDirs(self.pathMan.Paths['Deploy_API_Generated_Common'])
        self._SafeRemoveDirs(self.pathMan.Paths['Deploy_API_Generated_Platform'])

    def _SafeRemoveDirs(self, p):
        if not exists(p):
            self.logger.info('Folder {} does not exist. Skipped.'.format(p))
            return
        try:
            shutil.rmtree(p)
        except Exception as e:
            self.logger.warning('Failed to remove folder {} with error {}. Ignored.'.format(p, e))

