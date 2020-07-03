<xsl:stylesheet version="1.0"
            xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
            xmlns:wix="http://schemas.microsoft.com/wix/2006/wi">

  <xsl:output method="xml" indent="yes" />

  <xsl:strip-space elements="*"/>

  <xsl:template match="@*|node()">
    <xsl:copy>
      <xsl:apply-templates select="@*|node()"/>
    </xsl:copy>
  </xsl:template>
  
<!-- Add specific ID-tag to exe file for use in Product.wxs

  <xsl:template match="wix:File[contains(@Source, 'Arkade.GUI.exe') and not(contains(@Source, '.config'))]">
    <xsl:copy>
      <xsl:attribute name="Id">ApplicationExe</xsl:attribute>
      <xsl:apply-templates select="@*[name()!='Id'] | node()" />
    </xsl:copy>
  </xsl:template>
-->
  
  <xsl:template match="wix:File[contains(@Source, 'Arkade.GUI.exe') and not(contains(@Source, '.config'))]">
    <xsl:copy>
      <xsl:apply-templates select="@* | node()" />
      <wix:Shortcut Id="ApplicationStartMenuShortcut"
              Advertise="yes"
              Directory="ProgramMenuFolder"
              Name="Arkade 5"
              Icon="Arkade5_2.0.ico"
              WorkingDirectory="INSTALLFOLDER"/>
    </xsl:copy>
  </xsl:template>
  

  <xsl:template match="wix:Component[wix:File[contains(@Source, 'Arkade.GUI.exe') and not(contains(@Source, '.config'))]]">
    <xsl:copy>
      <xsl:apply-templates select="@* | node()" />
      <wix:RemoveFolder Id="RemoveApplicationProgramsFolder" Directory="ApplicationProgramsFolder" On="uninstall"/>
    </xsl:copy>
  </xsl:template>


  <!-- remove unwanted files -->
  <xsl:key name="kCompsToRemove" match="wix:Component[contains(wix:File/@Source, '.nupkg') 
                  or contains(wix:File/@Source, '.CodeAnalysisLog.xml')
                  or contains(wix:File/@Source, '.lastcodeanalysissucceeded')
           ]"
           use="@Id" />

  <xsl:template match="*[self::wix:Component or self::wix:ComponentRef]
                        [key('kCompsToRemove', @Id)]" />
  
</xsl:stylesheet>
