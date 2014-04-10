# //////////////////////////////////////////////////////////////////////
# //
# // Copyright (c) 2012 Audiokinetic Inc. / All Rights Reserved
# //
# //////////////////////////////////////////////////////////////////////
import sys, os, os.path, shutil, fnmatch, re, collections, logging, platform
from os.path import join, exists
import BuildUtil

def LineByLineReplaceFileByPattern(filePath, inPattern, outPattern):
	allLines = []
	with open(filePath) as f:
		allLines = f.readlines()
		f.close()

	for ll in range(len(allLines)):
		line = allLines[ll]
		match = re.search(inPattern, line)
		isTargetLine = (not match is None) and len(match.string) != 0
		if isTargetLine:
			matchString = match.string[match.start() : match.end()]
			allLines[ll] = re.sub(inPattern, outPattern, matchString)

	with open(filePath, 'w') as f:
		f.writelines(allLines)
		f.close()

def SimpleReplaceSubStringInLines(filePath, inPattern, outPattern):
	allLines = []
	with open(filePath) as f:
		allLines = f.readlines()
		f.close()

	for ll in range(len(allLines)):
		line = allLines[ll]
		allLines[ll] = line.replace(inPattern, outPattern)

	with open(filePath, 'w') as f:
		f.writelines(allLines)
		f.close()


class ApiRepairman(object):
	def __init__(self, pathMan):
		self.pathMan = pathMan
		self.logger = BuildUtil.CreateLogger(self.pathMan.Paths['Log'], __file__, self.__class__.__name__)

	def RepairPinvokeAPI(self):
		self.__FixApiPinvokeDeclarations()
		self.__BatchFixApiPinvokeCalls([self.pathMan.Paths['Deploy_API_Generated_Platform']])

	def MoveCommonAPI(self):
		'''Move common APIs to its folder'''

		generatedApiFiles = os.listdir(self.pathMan.Paths['Deploy_API_Generated_Platform'])

		commonApiFiles = []
		for f in generatedApiFiles:
			if not f in BuildUtil.PlatformDependentFilenames:
				commonApiFiles.append(f)

		if not os.path.exists(self.pathMan.Paths['Deploy_API_Generated_Common']):
			os.makedirs(self.pathMan.Paths['Deploy_API_Generated_Common'] )

		for f in commonApiFiles:
			src = os.path.join(self.pathMan.Paths['Deploy_API_Generated_Platform'], f)
			dest = os.path.join(self.pathMan.Paths['Deploy_API_Generated_Common'] , f)
			if os.path.exists(dest):
				try:
					os.remove(dest)
				except Exception as e:
					msg = 'Failed to remove destination {} before replacing. Error: {}'.format(dest, e)
					self.logger.error(msg)
					raise RuntimeError(msg)
			try:
				shutil.move(src, dest)
			except Exception as e:
				msg = 'Failed to move source to destination. Src: {}, Dest: {}. Error: {}'.format(src, dest, e)
				self.logger.error(msg)
				raise RuntimeError(msg)

	def MakePlatformApiSwitchable(self):
		self.__BatchMakeApiBypassable([self.pathMan.Paths['Deploy_API_Generated_Platform']], self.pathMan.PlatformName)
		self.__BatchMakeApiBypassable([self.pathMan.Paths['Deploy_API_Generated_Common']], 'Common')
		self.__BatchMakeApiBypassable([self.pathMan.Paths['Deploy_API_Handwritten']], 'Common')
		self.__BatchMakeApiBypassable([self.pathMan.Paths['Deploy_Examples']], 'Common')
		self.__BatchMakeApiFilenamesUnique([self.pathMan.Paths['Deploy_API_Generated_Platform']])


	def __BatchFixApiPinvokeCalls(self, dirPaths):
		for d in dirPaths:
			APIs = fnmatch.filter(os.listdir(d), '*.%s' % (BuildUtil.ApiExt))
			for api in APIs:
				if api == '%sPINVOKE.%s' % (self.pathMan.ProductName, BuildUtil.ApiExt):
					continue
				apiPath = os.path.join(d, api)
				self.__FixApiPinvokeCalls(apiPath)

	def __FixApiPinvokeCalls(self, apiPath):
		'''Update all PINVOKE calls with prefixed method names. It's simpler to use simple replace in this case than with regex.'''

		PinvokeCallPattern = '%sPINVOKE.' % self.pathMan.ProductName
		ReplacePattern = '%sPINVOKE.CSharp_' % self.pathMan.ProductName

		SimpleReplaceSubStringInLines(apiPath, PinvokeCallPattern, ReplacePattern)

	def __FixApiPinvokeDeclarations(self):
		'''Prepend CSharp_ prefix to all PINVOKE methods, required by Unity iOS, so we need consistency for all other platforms.'''

		methodDeclarationPattern = r'(?P<stubs>^[\s]*public[\s]+static[\s]+extern[\s]+)(?P<returnType>[\w\W*&\[\]\(\)\s]*?[\s]+)(?P<apiName>[\w\W]+[\s]*)(?P<tail>\([\s]*[\w\W*&,\[\]\(\)\s]*;)'
		ReplacePattern = r'\g<stubs>\g<returnType>CSharp_\g<apiName>\g<tail>'

		LineByLineReplaceFileByPattern(self.pathMan.Paths['Deploy_API_Generated_Platform_PInvoke'], methodDeclarationPattern, ReplacePattern)


	def __BatchMakeApiBypassable(self, dirPaths, platformKey, excludes=[]):
		for d in dirPaths:
			APIs = fnmatch.filter(os.listdir(d), '*.%s'%(BuildUtil.ApiExt))
			for api in APIs:
				if api in excludes:
					continue
				apiPath = os.path.join(d, api)
				self.__MakeApiBypassable(apiPath, platformKey)

	def __MakeApiBypassable(self, apiPath, platformKey):
		switchTail = '#endif // %s' % (BuildUtil.PlatformSwitches[platformKey])

		allLines = BuildUtil.ImportFile(apiPath)
		# Enclose allText with swiches
		switchHead = BuildUtil.PlatformSwitches[platformKey]
		# Example files may have exsiting switch. Replace instead of stack.
		switchHeadRegEx = re.compile(r'^[\s]*#if')
		switchTailRegEx = re.compile(r'^[\s]*#endif')
		hasSwitchHead = (not switchHeadRegEx.search(allLines[0]) is None) and (not switchTailRegEx.search(allLines[len(allLines)-1]) is None)
		if hasSwitchHead:
			allLines[0] = switchHead
			allLines[len(allLines)-1] = switchTail
		else:
			allLines = [switchHead] + allLines + [switchTail]
		BuildUtil.ExportFile(apiPath, allLines)

	def __BatchMakeApiFilenamesUnique(self, dirPaths):
		uniqueSuffix = self.pathMan.Paths['Platform']
		for d in dirPaths:
			APIs = fnmatch.filter(os.listdir(d), '*.%s'%(BuildUtil.ApiExt))
			for api in APIs:
				apiPath = os.path.join(d, api)
				self.__MakeApiFilenameUnique(apiPath, uniqueSuffix)

	def __MakeApiFilenameUnique(self, apiPath, suffix):
		pathComponents = os.path.splitext(apiPath)
		suffix = '_%s' % (suffix)
		newPath = pathComponents[0] + suffix + pathComponents[1]
		os.rename(apiPath, newPath)

	def InsertMultiArchPluginLoadDirectives(self, dllImportScript, outputScript):
		# Lib name = basename_arch

		if not exists(dllImportScript):
			msg = 'Failed to find DLL import script at: {}'.format(dllImportScript)
			self.logger.error(msg)
			raise RuntimeError(msg)
		lines = BuildUtil.ImportFile(dllImportScript)

		dllImportRegEx = re.compile(r'(?P<Leadspace>^[\s]*)(?P<ImportStart>\[[\s]*DllImport[\s]*\([\s]*"[\s]*)(?P<DllName>[\w]+)(?P<ImportEnd>[\s]*"[\s]*)')

		for i, l in enumerate(lines):
			match = dllImportRegEx.search(l)
			if match is None:
				continue

			dllNameToReplace = match.groupdict()['DllName']
			supportedArches = BuildUtil.SupportedArches[self.pathMan.PlatformName]

			# Modify and collect dllImportStatements
			dllImportStatements = collections.OrderedDict()
			for a in supportedArches:
				dllImportStatements[a] = l.replace(dllNameToReplace, '{}_{}'.format(dllNameToReplace, a)).rstrip() # Remove ending for later join.
			# Construct new lines
			newLines = []
			for arch, statement in dllImportStatements.items():
				switchMacro = BuildUtil.MultiArchSwitches[self.pathMan.PlatformName][arch]
				defineLine = '#elif {}'.format(switchMacro)
				newLines.append(defineLine)
				newLines.append(statement)
			newLines[0] = newLines[0].replace('#elif', '{}#if'.format(BuildUtil.SpecialChars['PosixLineEnd'])) # Hack back to correct leading conditional
			# Add default case
			defaultCaseLines = \
			[
				'#else',
				dllImportStatements[BuildUtil.DefaultArches[self.pathMan.PlatformName]],
				'#endif' + BuildUtil.SpecialChars['PosixLineEnd']
			]
			newLines += defaultCaseLines
			newLine = BuildUtil.SpecialChars['PosixLineEnd'].join(newLines)
			lines[i] = newLine
		BuildUtil.ExportFile(outputScript, lines)

def CopyWwiseSdkVersionFile(pathMan):
	src = join(pathMan.Paths['Wwise_SDK_Include'], 'AK', 'AkWwiseSDKVersion.h')
	dest = pathMan.Paths['Deploy']
	shutil.copy(src, dest)

def main(pathMan=None):
	logger = BuildUtil.CreateLogger(pathMan.Paths['Log'], __file__, main.__name__)

	if pathMan is None:
		pathMan = BuildUtil.Init()

	# Hack SWIG-generated Mono wrappers to maintain crossplatform consistency and make iOS work ...
	if not os.path.exists(pathMan.Paths['Deploy_API_Generated_Platform_PInvoke']):
		msg = 'Failed to find SWIG output APIs at: {}. Check under your platform deployment folder.'.format(pathMan.Paths['Deploy_API_Generated_Platform_PInvoke'])
		logger.error(msg)
		raise RuntimeError(msg)

	repairMan = ApiRepairman(pathMan)

	logger.info('Required step for iOS: Append CSharp_ to all PINVOKE API names for all platforms.')
	repairMan.RepairPinvokeAPI()

	logger.info('Avoid platform conflicts: Step 1: Copy common APIs to its own folder.')
	repairMan.MoveCommonAPI()

	# Must be done before we rename PINVOKE files.
	isMultiArchPlatform = pathMan.PlatformName in BuildUtil.SupportedArches.keys() and pathMan.PlatformName in BuildUtil.AkMultiArchPlatforms[platform.system()]
	if isMultiArchPlatform:
		logger.info('Insert switchable directives to PINVOKE script for loading multi-arch plugins.')
		pinvokeScript = pathMan.Paths['Deploy_API_Generated_Platform_PInvoke']
		repairMan.InsertMultiArchPluginLoadDirectives(pinvokeScript, pinvokeScript)
		# Android has two other handwritten dependencies.
		if pathMan.PlatformName == 'Android':
			unityJniDir = join(pathMan.Paths['Src_Platform'], 'UnityJNI')
			destDir = join(pathMan.Paths['Deploy_Dependencies'], pathMan.PlatformName)
			if not exists(destDir):
				try:
					os.makedirs(destDir)
				except:
					pass
			if exists(unityJniDir):
				dllImportScripts = fnmatch.filter(os.listdir(unityJniDir), '*.{}'.format(BuildUtil.ApiExt))
				for s in dllImportScripts:
					src = join(unityJniDir, s)
					dest = join(destDir, s)
					repairMan.InsertMultiArchPluginLoadDirectives(src, dest)
			else:
				isBuildingInUnityEditor = not exists(unityJniDir) and exists(destDir) and len(fnmatch.filter(os.listdir(destDir), '*.{}'.format(BuildUtil.ApiExt))) > 0
				if isBuildingInUnityEditor:
					logger.info('It\'s likely that we are building in Unity Editor where dependency folder replaced UnityJNI folder. Skipped.')

	logger.info('Avoid platform conflicts: Step 2: Enclose platform-specific APIs with Unity platform preprocessors.')
	logger.info('Avoid platform conflicts: Step 3: Rename platform-specific APIs with a unique suffix to avoid script name conflicts in Unity.')
	repairMan.MakePlatformApiSwitchable()

	# PS3-specific step
	if pathMan.PlatformName == 'PS3':
		sys.path.append(pathMan.Paths['Src_Platform'])
		import GeneratePS3LibraryStubs
		GeneratePS3LibraryStubs.main(pathMan)

	logger.info('Copy SDK version metadata.')
	CopyWwiseSdkVersionFile(pathMan)

	logger.info('SUCCESS. API binding for current platform can now coexist with other platforms in Unity Assets.')


if not __debug__:
	logging.debug('PS3: Need to add debug-guard to all custom sub-modules, against multiple-exec by ProDG. ProDG imports python modules in debug mode which runs that script as __main__ too. However, main module should not include guard in order to get executed.')
	if __name__ == '__main__':
		main()
