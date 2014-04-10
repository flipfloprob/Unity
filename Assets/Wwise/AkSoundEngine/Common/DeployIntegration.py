# //////////////////////////////////////////////////////////////////////
# //
# // Copyright (c) 2012 Audiokinetic Inc. / All Rights Reserved
# //
# //////////////////////////////////////////////////////////////////////
import argparse
from os.path import join, exists
import BuildUtil

class DeployManager(object):
	SupportedPlatforms = ['Android']

	def __init__(self, pathMan, Config=None, BuildDir=None, Arch=None):
		self.pathMan = pathMan
		self.Product = self.__CreatePluginBinaryPaths(Config, BuildDir, Arch)
		self.logger = BuildUtil.CreateLogger(pathMan.Paths['Log'], __file__, self.__class__.__name__)
		self.isDeployOnly = BuildDir == None

	def __CreatePluginBinaryPaths(self, Config, BuildDir, Arch):
		if self.pathMan.PlatformName == 'Android':
			return BuildUtil.NdkProduct(self.pathMan, Config, Arch)
		else:
			msg = 'Unsupported platform for post-build deployment: {}, available options: {}'.format(self.pathMan.PlatformName, ', '.join(BuildUtil.SupportedPlatforms))
			self.logger.error(msg)
			raise RuntimeError(msg)

	def Deploy(self):
		'''Copy plugin binaries from build folder to deploy source folder.'''
		self.Product.PreProcess()
		
		for b in self.Product.Binaries:
			src = join(self.Product.BuildDir, b)
			dest = join(self.Product.DeploySrcDir, b)
			self.logger.info('Copy plugin from build folder to deployment folder:\n     Source: {}\n     Destination: {}'.format(src, dest))
			BuildUtil.RecursiveReplace(src, dest)
			
		for b in self.Product.CommonBinaries:
			src = join(self.Product.BuildDir, b)
			dest = join(self.Product.CommonDeploySrcDir, b)
			self.logger.info('Copy plugin from build folder to deployment folder:\n     Source: {}\n     Destination: {}'.format(src, dest))
			BuildUtil.RecursiveReplace(src, dest)

		self.logger.info(BuildUtil.Messages['Prog_Success'])


def ParseAndValidateArgs(argStr=None):
	'''Read command line arguments and create variables.'''
	parser = argparse.ArgumentParser(description='Post-build event to deploy Wwise Unity Integartion')
	parser.add_argument('targetPlatform', metavar='platform', action='store', help='Target platform name, accepted options: {}.'.format(', '.join(DeployManager.SupportedPlatforms)))
	parser.add_argument('-a', '--arch', action='store', nargs=1, dest='arch', default=None, help='Target architecture name if available, default to None.')
	parser.add_argument('-b', '--builddir', action='store', nargs=1, dest='buildDir', default=None, help='Full path to build output folder. The build product is copied from this source location to destination deployemtn location.')
	parser.add_argument('-c', '--config', action='store', nargs=1, dest='config', default=None, help='Build configuration, available options: {}'.format(', '.join(BuildUtil.SupportedConfigs)))
	parser.add_argument('-u', '--unityprojroot', action='store', nargs=1, dest='unityProjectRoot', default=None, help='Path to Unity project root directory. If this option is set, then deploy the Integration to the Unity project assets folder.')
	
	if argStr == None:
		args = parser.parse_args()
	else: # for unittest
		args = parser.parse_args(argStr.split())

	# argparse outputs a list when nargs is specified.
	# So we take out the first arg in the list as the single arg.
	targetPlatform = args.targetPlatform

	arch = args.arch
	if not arch is None:
		arch = arch[0]
	buildDir = args.buildDir
	if not buildDir is None:
		buildDir = buildDir[0]
	config = args.config
	if not config is None:
		config = config[0]
	unityProjRoot = args.unityProjectRoot
	if not unityProjRoot is None:
		unityProjRoot = unityProjRoot[0]

	pathMan = BuildUtil.PathManager(targetPlatform, arch, unityProjRoot)

	# Verify and fail if necessary
	logger = BuildUtil.CreateLogger(pathMan.Paths['Log'], __file__, ParseAndValidateArgs.__name__)
	if targetPlatform in BuildUtil.SupportedArches.keys() and arch is None:
		msg = 'Target platform {} is a multi-architecture platform but no architecture is given (-a). Aborted.'.format(targetPlatform)
		logger.error(msg)
		raise RuntimeError(msg)
	if (not buildDir is None) and not exists(buildDir):
		msg = 'Failed to find build folder: {}. Aborted.'.format(buildDir)
		logger.error(msg)
		raise RuntimeError(msg)
	if config == None or not config in BuildUtil.SupportedConfigs:
		msg = 'Unsupported build configuration: {}. Aborted.'.format(config)
		logger.error(msg)
		raise RuntimeError(msg)
	
	return [pathMan, config, buildDir, arch]

def main():
	BuildUtil.CheckPythonVersion()
	[pathMan, Config, BuildDir, Arch] = ParseAndValidateArgs()
	deployMan = DeployManager(pathMan, Config, BuildDir, Arch)
	deployMan.Deploy()

if __name__ == '__main__':
	main()

