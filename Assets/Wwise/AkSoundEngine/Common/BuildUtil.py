# //////////////////////////////////////////////////////////////////////
# //
# // Copyright (c) 2012 Audiokinetic Inc. / All Rights Reserved
# //
# //////////////////////////////////////////////////////////////////////
import sys, os, os.path, shutil, subprocess, logging, logging.handlers, argparse, platform, json, copy
from os.path import abspath, dirname, join, basename, normpath, exists
from os import pardir as Parent

ScriptDir = abspath(dirname(__file__))
IsPython3 = sys.version_info[0] > 2
DefaultCodec = 'ascii'

def CheckPythonVersion():
	isPython27OrAbove = sys.version_info[:2] == (2, 7) or sys.version_info[0] > 2
	if not isPython27OrAbove:
		raise RuntimeError('Unexpected Python version: Expected 2.7 or above, but running Python{}.{}. Install an expected version from python.org and try again. Aborted.'.format(sys.version_info[0], sys.version_info[1]))

Version = '2013.2' # Update this by hand before releasing.

MetadataBaseName = 'BuildWwiseUnityIntegration'
DefaultLogFile = normpath(join(ScriptDir, Parent, Parent, 'Logs', '{}.log'.format(MetadataBaseName)))
IsVerbose = False
DefaultPrefFile = join(ScriptDir, '{}.json'.format(MetadataBaseName))

# Keys are output of platform.system()
SupportedPlatforms = \
{
	'Windows': ['Windows', 'Android', 'Metro', 'PS3', 'PS4', 'XBox360', 'XBoxOne', 'Vita'],
	'Darwin': ['Mac', 'iOS', 'Android']
}

# Multi-arch platforms that require swtiching dynamic loading targets using Script Define Symbols.
# Other multi-arch platforms have designated plugin folders for arches.
AkMultiArchPlatforms = \
{
	'Windows': ['Android', 'Metro', 'Vita', 'Windows'],
	'Darwin': ['Android']
}

SupportedArches = \
{
	'Android': ['armeabi-v7a'],
	'Metro': ['Metro_Win32', 'Metro_ARM'],
	'Windows': ['Win32', 'x64'],
	'Vita': ['SW', 'HW']
}

DefaultArches = \
{
	'Android': 'armeabi-v7a',
	'Metro': 'Metro_Win32',
	'Windows': 'Win32',
	'Vita': 'SW'
}

ApiExt = 'cs'

# Post-SWIG
PlatformSwitches = \
{
	'Windows': '#if UNITY_STANDALONE_WIN || (UNITY_EDITOR_WIN && UNITY_STANDALONE_WIN) || (UNITY_EDITOR_WIN && UNITY_XBOX360) || (UNITY_EDITOR_WIN && UNITY_XBOXONE) || (UNITY_EDITOR_WIN && UNITY_PS3) || (UNITY_EDITOR_WIN && UNITY_PS4) || (UNITY_EDITOR_WIN && UNITY_ANDROID) || (UNITY_EDITOR_WIN && UNITY_METRO) || (UNITY_EDITOR_WIN && UNITY_PSP2)',
	'Mac': '#if UNITY_STANDALONE_OSX || (UNITY_EDITOR_OSX && UNITY_STANDALONE_OSX) || (UNITY_EDITOR_OSX && UNITY_IOS) || (UNITY_EDITOR_OSX && UNITY_ANDROID)',
	'iOS': '#if UNITY_IOS && ! UNITY_EDITOR',
	'XBox360': '#if UNITY_XBOX360 && ! UNITY_EDITOR',
	'XBoxOne': '#if UNITY_XBOXONE && ! UNITY_EDITOR',
	'Android': '#if UNITY_ANDROID && ! UNITY_EDITOR',
	'AndroidMac': '#if UNITY_ANDROID && ! UNITY_EDITOR',
	'PS3': '#if UNITY_PS3 && ! UNITY_EDITOR',
	'PS4': '#if UNITY_PS4 && ! UNITY_EDITOR',
	'Metro': '#if UNITY_METRO && ! UNITY_EDITOR',
	'Vita': '#if UNITY_PSP2 && !UNITY_EDITOR',
	'Common': '#if ! (UNITY_DASHBOARD_WIDGET || UNITY_STANDALONE_LINUX || UNITY_WEBPLAYER || UNITY_WII || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY || UNITY_WP8) // Disable under unsupported platforms.'
}

PlatformDependentFilenames = \
[
	'AkSoundEngine.{}'.format(ApiExt),
	'AkSoundEnginePINVOKE.{}'.format(ApiExt),
	'AkPlatformInitSettings.{}'.format(ApiExt),
	'AkThreadProperties.{}'.format(ApiExt),
	'AkSpeakerVolumes.{}'.format(ApiExt),
	'AkMemPoolAttributes.{}'.format(ApiExt),
	'AkSoundQuality.{}'.format(ApiExt), # Windows only
	'AkSinkType.{}'.format(ApiExt), # Windows only
	'AkJobChainProperties.{}'.format(ApiExt), # PS3 only
	'AkGuid.{}'.format(ApiExt), # PS3 only
	'SWIGTYPE_p_unsigned_char.{}'.format(ApiExt), # PS3 only
	'AudioSessionCategory.{}'.format(ApiExt) # iOS only
]

MultiArchSwitches = \
{
	'Android': {'armeabi-v7a': 'AK_ARCH_ANDROID_ARMEABIV7A'},
	'Metro': {'Metro_Win32': 'AK_ARCH_METRO_WIN32', 'Metro_ARM': 'AK_ARCH_METRO_ARM'},
	'Windows': {'Win32': 'AK_ARCH_WINDOWS_WIN32', 'x64': 'AK_ARCH_WINDOWS_X64'},
	'Vita': { 'SW': 'AK_ARCH_VITA_SW', 'HW': 'AK_ARCH_VITA_HW' }
}

ExtraCallbackTypes = \
[
	', AK_Monitoring = 0x20000000', #Note: leading comma is used as separater from previous tail member
	', AK_Bank = 0x40000000', 
	', AK_AudioInterruption = 0x22000000'
]

Messages = \
	{
		'PlatformOptions': 'available options: {}.'.format(', '.join(SupportedPlatforms[platform.system()])),
		'Error_NoSdkDir': 'Failed to find $WWISESDK folder.',
		'Prog_Success': 'Build succeeded.',
		'Prog_Failed': 'Build failed.',
		'NotImplemented': 'Not implemented. Implement this in a subclass.',
		'CheckLogs': 'Check complete logs under: {}.'.format(normpath(dirname(DefaultLogFile)))
	}

SupportedConfigs = ['Debug', 'Profile', 'Release']

WwiseSdkEnvVar = 'WWISESDK'
AndroidNdkEnvVar = 'ANDROID_NDK_ROOT'
AndroidSdkEnvVar = 'ANDROID_HOME'
ApacheAntEnvVar = 'ANT_HOME'

# define target header and C++ declaration keywords
SpecialChars = {}
SpecialChars['Null'] = ''
SpecialChars['Comma'] = ','
SpecialChars['WhiteSpace'] = ' '
SpecialChars['StatementEnd'] = ';'
SpecialChars['PosixLineEnd'] = '\n'
SpecialChars['WindowsLineEnd'] = '\r\n'
SpecialChars['FunctionBrackets'] = {'Open': '(', 'Close': ')'}
SpecialChars['CurlyBrackets'] = {'Open': '{', 'Close': '}'}
SpecialChars['CComment'] = {'Open': '/*', 'Close': '*/'}
SpecialChars['CppComment'] = '//'


def CreateLogger(logPath, sourceFile, moduleName):
	'''Create a module-scope logger that writes comprehensive info to a log file and print error messages on screen.'''

	# Note: individual loggers can't share names otherwise they duplicate each other's logging messages.
	logger = logging.getLogger('{} ({})'.format(basename(sourceFile), moduleName))
	logger.setLevel(logging.DEBUG)

	# Avoid duplicated logs caused by duplicated handlers each time we create logger from a different module.
	if len(logger.handlers) > 0:
		return logger

	consoleHanlder = logging.StreamHandler()
	if IsVerbose:
		consoleHanlder.setLevel(logging.DEBUG)
	else:
		consoleHanlder.setLevel(logging.WARNING)
	formatter = logging.Formatter('Wwise: %(levelname)s: %(name)s: %(lineno)d: %(message)s')
	consoleHanlder.setFormatter(formatter)
	logger.addHandler(consoleHanlder)

	if not exists(logPath):
		logDir = os.path.split(logPath)[0]
		try:
			os.makedirs(normpath(logDir))
		except:
			pass

	isOnWindows = platform.system() == 'Windows'
	if isOnWindows:
		# Note: log file rotation is buggy on Windows due to file locking. Don't use it.
		fileHandler = logging.FileHandler(logPath)
	else:
		fileHandler = logging.handlers.TimedRotatingFileHandler(logPath, when='H', interval=1)
	fileHandler.setLevel(logging.DEBUG)
	formatter = logging.Formatter('%(asctime)s: %(levelname)s: %(name)s: %(lineno)d: %(message)s')
	fileHandler.setFormatter(formatter)
	logger.addHandler(fileHandler)

	return logger

def RunCommand(cmd):
	process = subprocess.Popen(cmd, shell=False, stdout=subprocess.PIPE, stderr=subprocess.PIPE, bufsize=0)
	(stdOut, stdErr) = process.communicate()

	if IsPython3:
		stdOut = stdOut.decode(DefaultCodec, 'ignore')
		stdErr = stdErr.decode(DefaultCodec, 'ignore')

	return (stdOut, stdErr, process.returncode)

def ImportFile(inputFile):
	rawLines = []
	with open(inputFile) as f:
		rawLines = f.readlines()
		f.close()

	return rawLines

def ExportFile(outputFile, outputLines):
	lines = copy.deepcopy(outputLines)
	# append line separators if none
	for ll in range(len(lines)):
		hasNoLinebreak = lines[ll].find(os.linesep) == -1 and lines[ll].find(SpecialChars['PosixLineEnd']) == -1
		if hasNoLinebreak:
			lines[ll] += SpecialChars['PosixLineEnd']

	with open(outputFile, 'w') as f:
		f.writelines(lines)
		f.close()

def RecursiveReplace(src, dest):
	logger = CreateLogger(DefaultLogFile, __file__, RecursiveReplace.__name__)

	# Remove destination
	if os.path.isdir(dest):
		try:
			shutil.rmtree(dest)
		except Exception as e:
			msg = 'Failed to remove destination folder: {} before replacing it. Error: {}. Aborted.'.format(dest, e)
			logger.error(msg)
			raise RuntimeError(msg)
	elif os.path.isfile(dest):
		try:
			os.remove(dest)
		except Exception as e:
			msg = 'Failed to remove destination file: {} before replacing it. Error: {}. Aborted.'.format(dest, e)
			logger.error(msg)
			raise RuntimeError(msg)
	else:
		# Not exist, proceed to copy.
		pass

	# Copy source to destination: Exclude SCM folders
	if os.path.isdir(src):
		try:
			shutil.copytree(src, dest, ignore=shutil.ignore_patterns('.svn', '.hg', '.git'))
		except Exception as e:
			msg = 'Failed to copy source to destination: Src: {}, Dest: {}. Error: {}. Aborted.'.format(src, dest, e)
			logger.error(msg)
			raise RuntimeError(msg)
	elif os.path.isfile(src):
		# Create dest parent folder if none exists
		destDir = os.path.dirname(dest)
		if not os.path.isdir(destDir):
			try:
				os.makedirs(destDir)
			except Exception as e:
				logger.warning('Failed to create folder destination {}. Error: {}. Ignored'.format(destDir, e))
		try:
			shutil.copy(src, dest)
		except Exception as e:
			msg = 'Failed to copy source to destination: Src: {}, Dest: {}. Error: {}. Aborted.'.format(src, dest, e)
			logger.error(msg)
			raise RuntimeError(msg)
	else:
		msg = 'Source is missing: {}. Aborted.'.format(src)
		logger.error(msg)
		raise RuntimeError(msg)

def WritePreferenceField(key, value):
	data = []
	if exists(DefaultPrefFile):
		with open(DefaultPrefFile) as f:
			dataStr = f.read()
			data = json.loads(dataStr)
			# Update field
			data[0][key] = value
	else:
		# Write as new data
		data = [{key:value}]

	dataStr = json.dumps(data, sort_keys=True, indent=4)
	with open(DefaultPrefFile, 'w') as f:
		f.write(dataStr)

def ReadPreferenceField(key):
	value = None
	data = []
	if exists(DefaultPrefFile):
		try:
			with open(DefaultPrefFile) as f:
				dataStr = f.read()
				data = json.loads(dataStr)
				# Update field
				if key in data[0]:
					value = data[0][key]
		except:
			return None
	return value


class PathManager(object):
	def __init__(self, PlatformName, Arch=None, wwiseSdkRootDir=None):
		'''Initialize all plugin related paths for other scripts to use.'''

		self.Paths = {}
		self.Paths['Root'] = self.NormJoin(os.path.abspath(os.path.dirname(__file__)), Parent, Parent)
		self.Paths['Log'] = DefaultLogFile
		self.logger = CreateLogger(self.Paths['Log'], __file__, self.__class__.__name__)

		if PlatformName == 'Android' and SpecialChars['WhiteSpace'] in self.Paths['Root']:
			msg = 'Unity Integration root path contains white spaces. Android build will fail. Consider removing all white spaces in Unity Integration root path: {}. Aborted.'.format(self.Paths['Root'])
			self.logger.error(msg)
			raise RuntimeError(msg)

		if not PlatformName in SupportedPlatforms[platform.system()]:
			msg = 'Unsupported target platform: {}, {}. Aborted.'.format(PlatformName, Messages['PlatformOptions'])
			self.logger.error(msg)
			raise RuntimeError(msg)

		self.PlatformName = PlatformName
		self.Arch = Arch

		self.Paths['Platform'] = self.PlatformName

		self.ProductName = 'AkSoundEngine'

		# NOTE: Assume this script stays in Common code folder!
		# We have to derive project dir here because on Android, Eclipse uses Win32 path, while Cygwin does not
		# while they use the same script.

		self.Paths['Src'] = self.NormJoin(self.Paths['Root'], self.ProductName)
		self.Paths['Src_Platform'] = self.NormJoin(self.Paths['Src'], self.Paths['Platform'])

		#
		# NOTE: Input arg overrides environment variable.
		#

		# Test if WWISESDK dir is defined.
		isWwiseSdkDirGiven = not wwiseSdkRootDir is None
		if isWwiseSdkDirGiven:
			os.environ[WwiseSdkEnvVar] = wwiseSdkRootDir
		else:
			if not WwiseSdkEnvVar in os.environ:
				msg = Messages['Error_NoSdkDir']
				self.logger.error(msg)
				raise RuntimeError(msg)

		# Test if WWISESDK dir exists
		# NOTE: On Windows, the valid path needs to be like D:\Wwise v2012.1.4 build 4260\SDK, without doublequotes.
		wwiseSdkRootDir = os.environ[WwiseSdkEnvVar]
		isUseCygwinStylePath = sys.platform == 'cygwin'
		if isUseCygwinStylePath:
			msg = 'Running under Cygwin Python. This would confuse SWIG. Try again using system Python instead. Aborted.'
			self.logger.error(msg)
			raise RuntimeError(msg)
		if not exists(wwiseSdkRootDir):
			msg = Messages['Error_NoSdkDir']
			self.logger.error(msg)
			raise RuntimeError(msg)

		self.Paths['Wwise_SDK'] = wwiseSdkRootDir
		self.Paths['Wwise_SDK_Include'] = self.NormJoin(self.Paths['Wwise_SDK'], 'include')
		self.Paths['Wwise_SDK_Samples'] = self.NormJoin(self.Paths['Wwise_SDK'], 'samples')

		self.Paths['Src_Common'] = self.NormJoin(self.Paths['Src'], 'Common')

		self.Paths['Doc'] = self.NormJoin(self.Paths['Root'], 'Documentation')
		self.Paths['Installer'] = self.NormJoin(self.Paths['Root'], 'Installers')

		self.Paths['Deploy'] = self.NormJoin(self.Paths['Root'], 'Deployment')
		self.Paths['Deploy_API'] = self.NormJoin(self.Paths['Deploy'], 'API')
		self.Paths['Deploy_API_Generated'] = self.NormJoin(self.Paths['Deploy_API'], 'Generated')
		self.Paths['Deploy_API_Generated_Common'] = self.NormJoin(self.Paths['Deploy_API_Generated'], 'Common')
		if sys.platform == 'darwin':
			self.Paths['Deploy_API_Generated_Desktop'] = self.NormJoin(self.Paths['Deploy_API_Generated'], 'Mac')
		else:
			self.Paths['Deploy_API_Generated_Desktop'] = self.NormJoin(self.Paths['Deploy_API_Generated'], 'Windows')
		self.Paths['Deploy_API_Generated_Platform'] = self.NormJoin(self.Paths['Deploy_API_Generated'], self.Paths['Platform'])
		self.Paths['Deploy_API_Generated_Platform_PInvoke'] = self.NormJoin(self.Paths['Deploy_API_Generated_Platform'], '{}PINVOKE.{}'.format(self.ProductName, ApiExt))
		self.Paths['Deploy_API_Generated_Platform_Module'] = self.NormJoin(self.Paths['Deploy_API_Generated_Platform'], '{}.{}'.format(self.ProductName, ApiExt))

		self.Paths['Deploy_API_Handwritten'] = self.NormJoin(self.Paths['Deploy_API'], 'Handwritten')
		self.Paths['Deploy_Plugins'] = self.NormJoin(self.Paths['Deploy'], 'Plugins', self.Paths['Platform'])

		self.Paths['Deploy_Examples'] = self.NormJoin(self.Paths['Deploy'], 'Examples')
		self.Paths['Deploy_Dependencies'] = self.NormJoin(self.Paths['Deploy'], 'Dependencies')

		self.Paths['SWIG'] = self.NormJoin(self.Paths['Root'], 'SWIG')
		self.Paths['SWIG_Interface'] = self.NormJoin(self.Paths['Src_Common'], 'SoundEngine.swig')
		self.Paths['SWIG_OutputApiDir'] = self.Paths['Deploy_API_Generated_Platform']
		self.Paths['SWIG_OutputCWrapper'] = self.NormJoin(self.Paths['Src_Platform'], 'SoundEngine_wrap.cxx')
		self.Paths['SWIG_WwiseSdkInclude'] = self.Paths['Wwise_SDK_Include']
		self.Paths['SWIG_PlatformInclude'] = self.Paths['Src_Platform']
		self.Paths['SWIG_CSharpModule'] = self.NormJoin(self.Paths['SWIG'], 'Lib', 'csharp')
		self.Paths['SWIG_WcharModule'] = self.NormJoin(self.Paths['SWIG_CSharpModule'], 'wchar.i')


	def ConvertWindowsToCygwinPath(self, path):
		return subprocess.check_output(['cygpath', '-u', path]).strip(SpecialChars['PosixLineEnd'])

	def ConvertCygwinToWindowsPath(self, path):
		return subprocess.check_output(['cygpath', '-w', path]).strip(SpecialChars['PosixLineEnd'])

	def NormJoin(self, *args):
		return normpath(join(*args))


def Init(argv=None):
	CheckPythonVersion()

	logger = CreateLogger(DefaultLogFile, __file__, Init.__name__)

	parser = argparse.ArgumentParser(description='Pre-build event to generate Unity script binding of Wwise SDK')
	parser.add_argument('targetPlatform', metavar='platform', action='store', help='Target platform name, available options: {}.'.format(', '.join(SupportedPlatforms[platform.system()])))
	parser.add_argument('-a', '--arch', nargs=1, dest='arch', default=None, help='The target architecture to build for certain platforms, available options: Windows: {}, Android: {}; Metro: {}, default to None if none given.'.format(', '.join(SupportedArches['Windows']), ', '.join(SupportedArches['Android']), ', '.join(SupportedArches['Metro'])))
	Undefined = 'Undefined'
	envWwiseSdkDir = Undefined
	if WwiseSdkEnvVar in os.environ:
		envWwiseSdkDir = os.environ[WwiseSdkEnvVar]
	parser.add_argument('-w', '--wwisesdkdir', nargs=1, dest='wwiseSdkDir', default=None, help='The Wwise SDK folder to build the Integration against. Required if -u is used. Abort if the specified path is not found; if no path specified in arguments, fall back to the environment variable WWISESDK. Current WWISESDK = {}. For Android, no white spaces are allowed in the Wwise SDK folder path.'.format(DefaultPrefFile, envWwiseSdkDir))
	parser.add_argument('-V', '--verbose', action='store_true', dest='isVerbose', default=False, help='Set the flag to show all log messages to console, default to unset (quiet).')

	if argv is None:
		args = parser.parse_args()
	else: # for unittest
		args = parser.parse_args(argv[1:])

	targetPlatform = args.targetPlatform
	targetArch = None
	if not args.arch is None:
		targetArch = args.arch[0]

	wwiseSdkDir = None
	if args.wwiseSdkDir is None:
		logger.warning('No Wwise SDK folder specified (-w). Fall back to use environment variable: {} = {}'.format(WwiseSdkEnvVar, envWwiseSdkDir))
	else:
		wwiseSdkDir = args.wwiseSdkDir[0]

	IsVerbose = args.isVerbose

	pathMan = PathManager(targetPlatform, targetArch, wwiseSdkDir)

	return pathMan


class PluginBuildProduct(object):
	def __init__(self, pathMan, Config):
		self.pathMan = pathMan

		self.Config = Config
		self.Binaries = [] # Files that go to the Deployment/Plugins/Platform/Arch/Configuration folder.
		self.CommonBinaries = [] # Files that go to the Deployment/Plugins/Platform folder.
		self.EditorScripts = []
		self.BuildDir = None
		self.DeploySrcDir = join(self.pathMan.Paths['Deploy_Plugins'], Config)
		self.CommonDeploySrcDir = self.pathMan.Paths['Deploy_Plugins']
		self.logger = CreateLogger(DefaultLogFile, __file__, self.__class__.__name__)

	def PreProcess(self):
		self.logger('Running from {}. Implement this in a subclass. Aborted.'.format(self.__class__.__name__))
		raise NotImplementedError()

class GenericIdeProduct(PluginBuildProduct):
	def __init__(self, pathMan, Config, BuildDir):
		PluginBuildProduct.__init__(self, pathMan, Config)

		self.BuildDir = BuildDir
		# self.FilePatterns = ['*.dll', '*.pdb']

	def CreateBinaryNames(self, Exts):
		self.Binaries = []
		for ext in Exts:
			self.Binaries.append('%s%s' % (self.pathMan.ProductName, ext))

class Win32Product(GenericIdeProduct):
	def __init__(self, pathMan, Config, BuildDir):
		GenericIdeProduct.__init__(self, pathMan, Config, BuildDir)
		self.CreateBinaryNames(['.dll', '.pdb'])
		self.DeploySrcDir = join(self.pathMan.Paths['Deploy_Plugins'], self.pathMan.PlatformName, Config)

class X64Product(Win32Product):
	def __init__(self, pathMan, Config, BuildDir):
		Win32Product.__init__(self, pathMan, Config, BuildDir)

class XBox360Product(GenericIdeProduct):
	def __init__(self, pathMan, Config, BuildDir):
		GenericIdeProduct.__init__(self, pathMan, Config, BuildDir)
		self.CreateBinaryNames(['.xex', '.pdb', '.def'])

class PS3Product(GenericIdeProduct):
	def __init__(self, pathMan, Config, BuildDir):
		GenericIdeProduct.__init__(self, pathMan, Config, BuildDir)
		self.CreateBinaryNames(['.prx', '.sprx', '_stub.a'])

class XcodeProduct(GenericIdeProduct):
	def __init__(self, pathMan, Config, BuildDir):
		GenericIdeProduct.__init__(self, pathMan, Config, BuildDir)

		self.Binaries = []

class MacProduct(XcodeProduct):
	def __init__(self, pathMan, Config, BuildDir):
		XcodeProduct.__init__(self, pathMan, Config, BuildDir)

		self.Binaries = [ '%s.bundle' % (self.pathMan.ProductName) ]

class iOSProduct(XcodeProduct):
	def __init__(self, pathMan, Config, BuildDir):
		XcodeProduct.__init__(self, pathMan, Config, BuildDir)

		self.Binaries = [ 'lib%s.a' % (self.pathMan.ProductName) ]

class NdkProduct(PluginBuildProduct):
	def __init__(self, pathMan, Config, Arch):
		PluginBuildProduct.__init__(self, pathMan, Config)

		self.Binaries = ['lib{}_{}.so'.format(self.pathMan.ProductName, self.pathMan.Arch)]
		self.CommonBinaries = ['AkAndroidIOInitializer.jar']
		# NOTE: NDK doesn't create build configuration subfolders, so we create them as deploy source folders by ourselves
		self.BuildDir = join(self.pathMan.Paths['Src_Platform'], 'obj', 'local', Arch)
		self.DeploySrcDir = join(self.pathMan.Paths['Deploy_Plugins'], Arch, Config)

		# NOTE: We don't deploy PostprocessBuildPlayer scripts anymore to avoid duplicated efforts in bank deployment
		# with StreamingAssets.
	def PreProcess(self):
		if (not self.BuildDir is None) and exists(self.BuildDir):
			# NDK uses separate buildDirs for .so and .jar. To ease things up, we copy .jar to .so folder to use just one buildDir as we do other platforms.
			for b in self.CommonBinaries:
				commonBinaryPath = join(self.pathMan.Paths['Src_Platform'], 'bin', b)
				dest = join(self.BuildDir, b)
				if exists(dest):
					try:
						os.remove(dest)
					except Exception as e:
						msg = 'Failed to remove outdated Android common binary file: {}, Error: {}. Aborted.'.format(dest, e)
						self.logger.error(msg)
						raise RuntimeError(msg)
				try:
					shutil.move(commonBinaryPath, self.BuildDir)
				except Exception as e:
					msg = 'Failed to move Android common binary file: {}, Error: {}. Aborted.'.format(commonBinaryPath, e)
					self.logger.error(msg)
					raise RuntimeError(msg)
		else:
			msg = 'Failed to find Android build folder: {}. Aborted.'.format(self.BuildDir)
			self.logger.error(msg)
			raise RuntimeError(msg)

class MetroProduct(Win32Product):
	def __init__(self, pathMan, Config, BuildDir):
		Win32Product.__init__(self, pathMan, Config, BuildDir)
		self.DeploySrcDir = join(self.pathMan.Paths['Deploy_Plugins'], self.pathMan.PlatformName, Config)

if __name__ == '__main__':
	pass
