# //////////////////////////////////////////////////////////////////////
# //
# // Copyright (c) 2012 Audiokinetic Inc. / All Rights Reserved
# //
# //////////////////////////////////////////////////////////////////////
import os, sys
from os.path import join, exists
import BuildUtil
import PrepareSwigInput, PostprocessSwigOutput
from BuildInitializer import BuildInitializer

class SwigCommand(object):
	def __init__(self, pathMan):
		self.pathMan = pathMan

		self.EXE = join(self.pathMan.Paths['SWIG'], 'swig.exe')
		self.platformDefines = []
		self.compilerDefines = []
		self.outputLanguage = ['-csharp']
		self.dllName = []
		self.Includes = []

		self.logger = BuildUtil.CreateLogger(pathMan.Paths['Log'], __file__, self.__class__.__name__)

	def CreatePlatformCommand(self):
		PlatformName = self.pathMan.PlatformName
		Arch = self.pathMan.Arch
		if PlatformName == 'Windows' and Arch == 'Win32':
			return Win32SwigCommand(self.pathMan)
		elif PlatformName == 'Windows' and Arch == 'x64':
			return X64SwigCommand(self.pathMan)
		elif PlatformName == 'XBox360':
			return XBox360SwigCommand(self.pathMan)
		elif PlatformName == 'XBoxOne':
			return XBoxOneSwigCommand(self.pathMan)
		elif PlatformName == 'Mac':
			return MacSwigCommand(self.pathMan)
		elif PlatformName == 'iOS':
			return iOSSwigCommand(self.pathMan)
		elif PlatformName == 'Android':
			return AndroidSwigCommand(self.pathMan)
		elif PlatformName == 'PS3':
			return PS3SwigCommand(self.pathMan)
		elif PlatformName == 'PS4':
			return PS4SwigCommand(self.pathMan)
		elif PlatformName == 'Metro' and Arch == 'Metro_Win32':
			return MetroWin32SwigCommand(self.pathMan)
		elif PlatformName == 'Metro' and Arch == 'Metro_ARM':
			return MetroARMSwigCommand(self.pathMan)
		elif PlatformName == 'Vita':
			return VitaSwigCommand(self.pathMan)		
		else:
			return None

	def Run(self):
		exe = [self.EXE]

		output = ['-o', self.pathMan.Paths['SWIG_OutputCWrapper']]
		includes = ['-I{}'.format(self.pathMan.Paths['SWIG_WwiseSdkInclude']), '-I{}'.format(self.pathMan.Paths['SWIG_PlatformInclude']), '-I{}'.format(self.pathMan.Paths['SWIG_CSharpModule'])] + self.Includes
		links = ['-l{}'.format(self.pathMan.Paths['SWIG_WcharModule'])]
		excludeExceptionHandling = ['-DSWIG_CSHARP_NO_EXCEPTION_HELPER', '-DSWIG_CSHARP_NO_WSTRING_HELPER', '-DSWIG_CSHARP_NO_STRING_HELPER']
		outdir = ['-outdir', self.pathMan.Paths['SWIG_OutputApiDir']]
		inputLanguage = ['-c++']
		interface = [self.pathMan.Paths['SWIG_Interface']]

		if not exists(self.pathMan.Paths['SWIG_OutputApiDir']):
			try:
				os.makedirs(self.pathMan.Paths['SWIG_OutputApiDir'])
			except:
				pass

		cmd = exe + output + includes + links + self.platformDefines + self.compilerDefines + excludeExceptionHandling + self.dllName + outdir + inputLanguage + self.outputLanguage + interface

		self.logger.debug(' '.join(cmd))

		try:
			(stdOut, stdErr, returnCode) = BuildUtil.RunCommand(cmd)
			self.logger.debug('stdout:\n{}'.format(stdOut))
			self.logger.debug('stderr:\n{}'.format(stdErr))
			isCmdFailed = returnCode != 0
			if isCmdFailed:
				msg = 'SWIG failed with return code: {}, Command: {}'.format(returnCode, ' '.join(cmd))
				raise RuntimeError(msg)
		except Exception as err:
			# Note: logger.exception() is buggy in py3.
			self.logger.error(err)
			raise RuntimeError(err)


class Win32SwigCommand(SwigCommand):
	def __init__(self, pathMan):
		SwigCommand.__init__(self, pathMan)

		self.platformDefines = ['-DWIN32', '-D_WIN32']

class X64SwigCommand(SwigCommand):
	def __init__(self, pathMan):
		SwigCommand.__init__(self, pathMan)

		self.platformDefines = ['-DWIN64', '-D_WIN64']

class XBox360SwigCommand(SwigCommand):
	def __init__(self, pathMan):
		SwigCommand.__init__(self, pathMan)

		self.platformDefines = ['-DXBox360']

class XBoxOneSwigCommand(SwigCommand):
	def __init__(self, pathMan):
		SwigCommand.__init__(self, pathMan)

		self.platformDefines = ['-D_XBOX_ONE']

class GccSwigCommand(SwigCommand):
	def __init__(self, pathMan):
		SwigCommand.__init__(self, pathMan)
		self.compilerDefines = ['-D__GNUC__', '-DGCC_HASCLASSVISIBILITY']

class AppleSwigCommand(GccSwigCommand):
	def __init__(self, pathMan):
		GccSwigCommand.__init__(self, pathMan)
		self.EXE = 'swig'
		self.platformDefines = ['-D__APPLE__', '-DAK_APPLE']

	def _InitPlatformSdkInfo(self, platformSdkName, newSdks, oldSdks, headerSwitch):
		# For UnitTest that runs outside of Xcode on Mac OS X Mountain Lion
		SdkRoot = None

		isSdkRootEnvVarExist = 'SDKROOT' in os.environ and os.environ['SDKROOT'] != ''
		if isSdkRootEnvVarExist:
			SdkRoot = os.environ['SDKROOT']
		else: # search first valid Mac SDK
			self.logger.debug('Detected: Running in command line outside Xcode IDE environment. Proceed to find SDKROOT ourselves.')

			XcodeDeveloperDir = None
			cmd = ['/usr/bin/xcode-select', '--print-path']
			self.logger.debug(' '.join(cmd))
			try:
				(stdOut, stdErr, returnCode) = BuildUtil.RunCommand(cmd)
				outMsgs = stdOut.split(BuildUtil.SpecialChars['PosixLineEnd'])
				errMsgs = stdErr.split(BuildUtil.SpecialChars['PosixLineEnd'])
				for o in outMsgs:
					self.logger.debug('stdout: {}'.format(o))
				for e in errMsgs:
					self.logger.debug('stderr: {}'.format(e))
				isCmdFailed = returnCode != 0
				if isCmdFailed:
					msg = 'Command failed with return code: {}, Command: {}'.format(returnCode, ' '.join(cmd))
					raise RuntimeError(msg)
			except Exception as err:
				# Note: logger.exception() is buggy in py3.
				self.logger.error(err)
				raise RuntimeError(err)
			XcodeDeveloperDir = outMsgs[0]

			latestSdkRoot = None
			sdks = newSdks + oldSdks # new SDKs have higher priority.
			for s in sdks:
				path = '{}/Platforms/{}.platform/Developer/SDKs/{}'.format(XcodeDeveloperDir, platformSdkName, s)
				if exists(path):
					latestSdkRoot = path
					break
			SdkRoot = latestSdkRoot

		assert SdkRoot != None
		
		# Add new-SDK-based defines
		for sdk in newSdks:
			if sdk in SdkRoot:
				self.platformDefines.append(headerSwitch)
				break

		sdkIncludeDir = join(SdkRoot, 'usr', 'include')
		self.Includes = ['-I%s' % (sdkIncludeDir)]

class MacSwigCommand(AppleSwigCommand):
	def __init__(self, pathMan):
		AppleSwigCommand.__init__(self, pathMan)

		self.EXE = 'swig'
		self.platformDefines.append('-D__i386__') # Does not have to be 64bit, just to suppress error for SWIG.

		MacOSXSdks_MavericksAndAbove = ['MacOSX10.9.sdk']
		MacOSXSdks_MoutainLionAndBelow = ['MacOSX10.8.sdk', 'MacOSX10.7.sdk', 'MacOSX10.6.sdk']
		self._InitPlatformSdkInfo(platformSdkName='MacOSX', newSdks=MacOSXSdks_MavericksAndAbove, oldSdks=MacOSXSdks_MoutainLionAndBelow, headerSwitch='-DAK_MAC_OS_X_10_9')

class iOSSwigCommand(AppleSwigCommand):
	def __init__(self, pathMan):
		AppleSwigCommand.__init__(self, pathMan)

		self.EXE = 'swig'
		self.platformDefines += ['-DAK_IOS', '-DTARGET_OS_IPHONE', '-DTARGET_OS_EMBEDDED', '-D__arm__'] # No need for -D__arm64__. __arm__ is good enough for fixing header includes on iOS7.
		self.dllName = ['-dllimport', '__Internal']

		iOSSdks_7AndAbove = ['iPhoneOS7.0.sdk']
		iOSSdks_6AndBelow = ['iPhoneOS6.1.sdk', 'iPhoneOS6.0.sdk', 'iPhoneOS5.1.sdk', 'iPhoneOS5.0.sdk']
		self._InitPlatformSdkInfo(platformSdkName='iPhoneOS', newSdks=iOSSdks_7AndAbove, oldSdks=iOSSdks_6AndBelow, headerSwitch='-DAK_IOS_7')

class AndroidSwigCommand(GccSwigCommand):
	def __init__(self, pathMan):
		GccSwigCommand.__init__(self, pathMan)

		isOnMac = sys.platform == 'darwin'
		if isOnMac:
			self.EXE = '/usr/local/bin/swig'
		else:
			self.EXE = os.path.join(self.pathMan.Paths['SWIG'], 'swig.exe')

		self.platformDefines += ['-D__ANDROID__', '-DAK_ANDROID']
		self.dllName = ['-dllimport', self.pathMan.ProductName]

		# NOTE: hard code supported platform here.
		AndroidSDKVersion = 'android-9'
		Arch = 'arch-arm'
		self.Includes = ['-I%s' % (os.path.join(os.environ['ANDROID_NDK_ROOT'], 'platforms', AndroidSDKVersion, Arch, 'usr', 'include'))]

class PS3SwigCommand(GccSwigCommand):
	def __init__(self, pathMan):
		GccSwigCommand.__init__(self, pathMan)

		self.platformDefines += ['-D__PPU__']

class PS4SwigCommand(GccSwigCommand):
	def __init__(self, pathMan):
		GccSwigCommand.__init__(self, pathMan)
		
		self.platformDefines += ['-D__ORBIS__', '-DSN_TARGET_ORBIS', '-D__SCE__','-DAK_PS4']
		self.Includes = ['-I%s' % (os.path.join(os.environ['SCE_ORBIS_SDK_DIR'], 'target', 'include')), '-I%s' % (os.path.join(os.environ['SCE_ORBIS_SDK_DIR'], 'target', 'include_common')), '-I%s' % (os.path.join(os.environ['SCE_ORBIS_SDK_DIR'], 'host_tools', 'lib', 'clang', 'include'))]

class MetroWin32SwigCommand(SwigCommand):
	def __init__(self, pathMan):
		SwigCommand.__init__(self, pathMan)

		self.platformDefines = ['-DAK_METRO', '-DWIN32', '-DWINAPI_FAMILY', '-D_M_IX86']

class MetroARMSwigCommand(SwigCommand):
	def __init__(self, pathMan):
		SwigCommand.__init__(self, pathMan)

		# Take result from #if !WINAPI_FAMILY_PARTITION(WINAPI_PARTITION_DESKTOP) in Windows/AkTypes.h
		self.platformDefines = ['-DAK_METRO', '-DWINAPI_FAMILY', '-D_M_ARM', '-DAK_USE_METRO_API', '-DAK_USE_THREAD_EMULATION']

class VitaSwigCommand(SwigCommand):
	def __init__(self, pathMan):
		SwigCommand.__init__(self, pathMan)	
		self.platformDefines += ['-D__SCE__', '-D__arm__','-DAK_VITA','-DAK_SUPPORT_WCHAR']		
		
def main():
	pathMan = BuildUtil.Init()
	logger = BuildUtil.CreateLogger(pathMan.Paths['Log'], __file__, main.__name__)

	logger.info('Using WWISESDK: {}'.format(pathMan.Paths['Wwise_SDK']))

	logger.info('Remove existing API bindings to avoid conflicts.')
	initer = BuildInitializer(pathMan)
	initer.Initialize()

	logger.info('Prepare SDK header blob for generating API bindings, and platform source code for building plugin.')
	PrepareSwigInput.main(pathMan)

	logger.info('Generate API binding for Wwise SDK.')
	swigCmd = SwigCommand(pathMan).CreatePlatformCommand()
	swigCmd.Run()

	logger.info('Postprocess generated API bindings to make platform and architecture switchable and make iOS API work in Unity.')
	PostprocessSwigOutput.main(pathMan)

	logger.info('SUCCESS. API binding generated under {}.'.format(pathMan.Paths['Deploy_API_Generated']))


if __name__ == '__main__':
	main()
