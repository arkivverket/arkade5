def makeBuild = {
  bat "nuget restore src\\Arkivverket.Arkade.sln"
  changeAsmVer versionPattern: "${VERSION_NUMBER}"
  // nuget restore ?
  // use dotnet msbuild command?
  bat "\"${tool 'MSBuild2022'}\\MSBuild.exe\" /p:Configuration=Release /p:Version=${VERSION_NUMBER} /p:AssemblyVersion=${VERSION_NUMBER} src\\Arkivverket.Arkade.GUI\\Arkivverket.Arkade.GUI.csproj "
  //bat "\"${tool 'MSBuild2022'}\\MSBuild.exe\" /p:Configuration=Release /p:Version=${VERSION_NUMBER} /p:AssemblyVersion=${VERSION_NUMBER} src\\Arkivverket.Arkade.CLI\\Arkivverket.Arkade.CLI.csproj "
  bat "\"${tool 'MSBuild2022'}\\MSBuild.exe\" /p:Configuration=Release /p:Version=${VERSION_NUMBER} /p:AssemblyVersion=${VERSION_NUMBER} src\\Setup\\Setup.wixproj "
  archiveArtifacts artifacts: "${GUI_ARTIFACTFILE_FULLNAME}"
  //archiveArtifacts artifacts: "${CLI_ARTIFACTFILE_FULLNAME}"
  bat "copy ${GUI_ARTIFACTFILE_FULLNAME} ${GUI_INSTALLFILE_FULLNAME}"
  //bat "copy ${CLI_ARTIFACTFILE_FULLNAME} ${CLI_INSTALLFILE_FULLNAME}"
}

pipeline {
  agent any
  environment {
    PACKAGE_NAME = "Arkade5"
    PROJECT_PRIMARY = "Arkivverket.Arkade"
    GUI_ARTIFACTFILE_FULLNAME = "src\\Setup\\bin\\Release\\NORELEASE_Arkade5-0.0.0_RC0.msi"
    //CLI_ARTIFACTFILE_FULLNAME = "src\\Setup\\bin\\Release\\net6.0\\publish\\NORELEASE_Arkade5CLI-0.0.0_RC0"
    INSTALLFILE_BASEDIRECTORY = "C:\\inetpub\\sites\\download\\arkade\\"
  }
  stages {
    stage("MakeReleaseCandidate") {
      when {
        branch "release/*"
      }
      environment {
        VERSION_NUMBER = "0.0.0.${currentBuild.getNumber()}"      
        RC_VERSION = BRANCH_NAME.minus("release/")
        RC_NUMBER = currentBuild.getNumber()
        INSTALLFILE_FILENAME = "NORELEASE_${PACKAGE_NAME}-${RC_VERSION}_RC${RC_NUMBER}"
        GUI_INSTALLFILE_FULLNAME = "${INSTALLFILE_BASEDIRECTORY}release-candidates\\${INSTALLFILE_FILENAME}.msi"
        //CLI_INSTALLFILE_FULLNAME = "${INSTALLFILE_BASEDIRECTORY}release-candidates\\${INSTALLFILE_FILENAME}.zip"
      }
      steps {
        script {makeBuild()}
      }
    }
    stage("MakeRelease") {
      when {
        buildingTag()
      }
      environment {
        VERSION_NUMBER = "${TAG_NAME}"
        INSTALLFILE_FILENAME = "${PACKAGE_NAME}-${TAG_NAME}"
        GUI_INSTALLFILE_FULLNAME = "${INSTALLFILE_BASEDIRECTORY}releases\\${INSTALLFILE_FILENAME}.msi"
        //CLI_INSTALLFILE_FULLNAME = "${INSTALLFILE_BASEDIRECTORY}releases\\${INSTALLFILE_FILENAME}.zip"
      }
      steps {
        script {makeBuild()}
      }
    }
  }
}

// Required Jenkins config
//
// Jenkins Git plugin: Discover branches, Discover tags
// SCM API Plugin - Filter by name (with regular expression): release\/v\d+\.\d+\.\d+|v\d+\.\d+\.\d+
// Basic Branch Build Strategies Plugin: Regular branches, Tags
//
// The name filter regex ensures that RC_VERSION (in stage MakeReleaseCandidate) or TAG_NAME (in stage MakeRelease)
// has a (3-part) version number format, e.g. 1.0.0 from a branch named release/1.0.0 or a tag named 1.0.0
