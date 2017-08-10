<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns="http://www.arkivverket.no/standarder/addml"
    xmlns:xs="http://www.w3.org/2001/XMLSchema"
    xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
    version="1.0">

<!--
        Denne transformering skjer fra en NoarkIH.xml og gjør denne om til en addml.xml.
        
        Foreløpig er det en feil ved at flere av elementene får med attributtet xlmns="".
-->
    <xsl:output method="xml" version="1.0"
        encoding="UTF-8" indent="yes"/>
    <!--<xsl:variable name="path">
        <xsl:value-of select="'DATA/'"/>
    </xsl:variable>-->
    
    <xsl:template match="/">
        <xsl:apply-templates/>
    </xsl:template>
    
    <xsl:template match="NOARK.IH">
        <addml xmlns="http://www.arkivverket.no/standarder/addml"
            xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
            xsi:schemaLocation="http://www.arkivverket.no/standarder/addml addml.xsd">
            <dataset>
                <reference>
                        <xsl:apply-templates select="EKSPORTINFO"/>
                </reference>
                <flatFiles>
                    <xsl:apply-templates select="TABELLINFO"/>
                    <flatFileDefinitions>
                        <xsl:for-each select="TABELLINFO">
                            <xsl:apply-templates select="ATTRIBUTTER"/>
                        </xsl:for-each>
                    </flatFileDefinitions>
                    <structureTypes>
                        <flatFileTypes>
                            <flatFileType name="filref">
                                <charset>ISO-8859-1</charset>
                                <fixedFileFormat/>
                            </flatFileType>
                        </flatFileTypes>
                        <recordTypes>
                            <recordType name="recref"/>
                        </recordTypes>
                        <fieldTypes>
                            <fieldType name="string">
                                <dataType>string</dataType>
                            </fieldType>
                            <fieldType name="integer">
                                <dataType>integer</dataType>
                            </fieldType>
                            <fieldType name="date8">
                                <dataType>date</dataType>
                                <fieldFormat>yyyymmdd</fieldFormat>
                            </fieldType>
                            <fieldType name="time">
                                <dataType>time</dataType>
                                <fieldFormat>hhmm</fieldFormat>
                            </fieldType>
                            <fieldType name="boolean">
                                <dataType>boolean</dataType>
                                <fieldFormat>1/0</fieldFormat>
                            </fieldType>
                        </fieldTypes>
                    </structureTypes>
                    <flatFileProcesses flatFileReference="NOARKSAK">
                        <processes>
                            <process name="Analyse_CountRecords"/>
                            <process name="Control_NumberOfRecords"/>
                        </processes>
                        <recordProcesses definitionReference="NOARKSAK">
                            <processes>
                                <process name="Analyse_CountRecordDefinitionOccurences"/>
                                <process name="Control_Key"/>
                                <process name="Control_ForeignKey"/>
                            </processes>
                            <fieldProcesses definitionReference="SA.DATO">
                                <processes>
                                    <process name="Control_DataFormat"/>
                                </processes>
                            </fieldProcesses>
                        </recordProcesses>
                    </flatFileProcesses>
                </flatFiles>
            </dataset>
        </addml>
    </xsl:template>
    
    <xsl:template match="EKSPORTINFO">
        <context>
            <xsl:choose>
            <xsl:when test="EI.ARKSKAPER">
                <additionalElements>
                    <additionalElement name="agents">
                        <additionalElements>
                            <additionalElement name="agent">
                                <additionalElements>
                                    <additionalElement name="role">
                                        <value>recordCreator</value>
                                    </additionalElement>
                                    <additionalElement name="type">
                                        <value>institution</value>
                                    </additionalElement>
                                    <additionalElement name="name">
                                        <value><xsl:value-of select="EI.ARKSKAPER"/></value>
                                    </additionalElement>
                                </additionalElements>
                            </additionalElement>
                        </additionalElements>
                    </additionalElement>
                </additionalElements>
            </xsl:when>
            <xsl:when test="EI.SYSTEMNAVN">
                <additionalElements>
                    <additionalElement name="system">
                        <additionalElements>
                            <additionalElement name="systemType">
                                <value>Noark 4</value>
                            </additionalElement>
                            <additionalElement name="systemNavn">
                                <value><xsl:value-of select="EI.SYSTEMNAVN"/></value>
                            </additionalElement>
                        </additionalElements>
                    </additionalElement>
                </additionalElements>
            </xsl:when>
        </xsl:choose>
        </context>
        <content>
            <xsl:choose>
            <xsl:when test="EI.FRADATO">
                <additionalElements>
                    <additionalElement name="archivalPeriod">
                        <additionalElements>
                            <additionalElement name="startDate">
                                <value><xsl:value-of select="EI.FRADATO"/></value>
                            </additionalElement>
                            <additionalElement name="endDate">
                                <value><xsl:value-of select="EI.TILDATO"/></value>
                            </additionalElement>
                            <additionalElement name="type">
                                <value>Noark-4</value>
                            </additionalElement>
                        </additionalElements>
                    </additionalElement>
                </additionalElements>
            </xsl:when>
            <xsl:when test="EI.PRODDATO">
                <additionalElements>
                    <additionalElement name="archivalDataset">
                        <additionalElements>
                            <additionalElement name="date">
                                <value><xsl:value-of select="EI.PRODDATO"/></value>
                            </additionalElement>
                            <additionalElement name="type">
                                <value>Noark-4</value>
                            </additionalElement>
                        </additionalElements>
                    </additionalElement>
                </additionalElements>
            </xsl:when>
            <xsl:otherwise>
                <comment>Hva har skjedd nå?</comment>
            </xsl:otherwise>
        </xsl:choose>
        </content>
    </xsl:template>
    
    <xsl:template match="TABELLINFO">
        <xsl:variable name="filnavn" select="TI.TABELL"/>
        <flatFile definitionReference="{$filnavn}" name="{$filnavn}">
            <!--<value><xsl:value-of select="TI.TABELL"/></value>-->
            <properties>
                <property name="fileName">
                    <value>DATA\<xsl:value-of select="FIL/TI.FILNAVN"/></value>
                </property>
                <property name="numberOfRecords">
                    <value><xsl:value-of select="FIL/TI.ANTPOSTER"/></value>
                </property>
            </properties>
        </flatFile>
    </xsl:template>
    
    <xsl:template match="ATTRIBUTTER">
        <xsl:variable name="filnavn" select="../TI.TABELL"/>
        <!--<flatFileDefinitions>-->
            <flatFileDefinition name="{$filnavn}" typeReference="filref">
                <recordDefinitions>
                    <recordDefinition name="{$filnavn}">
                        <xsl:choose>
                            <xsl:when test="$filnavn='NOARKSAK'">
                                <keys>
                                    <key name="prim_{$filnavn}">
                                        <primaryKey/>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="SA.ID"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                    <key name="alt_{$filnavn}">
                                        <alternateKey/>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="SA.ARKDEL"/>
                                            <fieldDefinitionReference name="SA.SAAR"/>
                                            <fieldDefinitionReference name="SA.SEKNR"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                    <key name="for01_admindel_{$filnavn}">
                                        <foreignKey>
                                            <flatFileDefinitionReference name="ADMINDEL">
                                                <recordDefinitionReferences>
                                                    <recordDefinitionReference name="ADMINDEL">
                                                        <fieldDefinitionReferences>
                                                            <fieldDefinitionReference name="AI.ID"/>
                                                        </fieldDefinitionReferences>
                                                    </recordDefinitionReference>
                                                </recordDefinitionReferences>
                                            </flatFileDefinitionReference>
                                            <relationType>n:1</relationType>
                                        </foreignKey>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="SA.ADMID"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                    <key name="for02_pernavn_ansv_{$filnavn}">
                                        <foreignKey>
                                            <flatFileDefinitionReference name="PERNAVN">
                                                <recordDefinitionReferences>
                                                    <recordDefinitionReference name="PERNAVN">
                                                        <fieldDefinitionReferences>
                                                            <fieldDefinitionReference name="PN.ID"/>
                                                        </fieldDefinitionReferences>
                                                    </recordDefinitionReference>
                                                </recordDefinitionReferences>
                                            </flatFileDefinitionReference>
                                            <relationType>n:1</relationType>
                                        </foreignKey>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="SA.ANSVID"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                    <key name="for03_pernavn_utlev_{$filnavn}">
                                        <foreignKey>
                                            <flatFileDefinitionReference name="PERNAVN">
                                                <recordDefinitionReferences>
                                                    <recordDefinitionReference name="PERNAVN">
                                                        <fieldDefinitionReferences>
                                                            <fieldDefinitionReference name="PN.ID"/>
                                                        </fieldDefinitionReferences>
                                                    </recordDefinitionReference>
                                                </recordDefinitionReferences>
                                            </flatFileDefinitionReference>
                                            <relationType>n:1</relationType>
                                        </foreignKey>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="SA.UTLTIL"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                </keys>
                            </xsl:when>
                            <xsl:when test="$filnavn='JFSAK'">
                                <keys>
                                    <key name="prim_{$filnavn}">
                                        <primaryKey/>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="JF.ID"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                </keys>
                            </xsl:when>
                            <xsl:when test="$filnavn='KLASSERING'">
                                <keys>
                                    <key name="prim_{$filnavn}">
                                        <primaryKey/>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="KL.SAID"/>
                                            <fieldDefinitionReference name="KL.ORDNPRI"/>
                                            <fieldDefinitionReference name="KL.ORDNVER"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                    <key name="for04_klass_ordnprins_{$filnavn}">
                                        <foreignKey>
                                            <flatFileDefinitionReference name="ORDNPRINS">
                                                <recordDefinitionReferences>
                                                    <recordDefinitionReference name="ORDNPRINS">
                                                        <fieldDefinitionReferences>
                                                            <fieldDefinitionReference name="OP.ORDNPRI"/>
                                                        </fieldDefinitionReferences>
                                                    </recordDefinitionReference>
                                                </recordDefinitionReferences>
                                            </flatFileDefinitionReference>
                                            <relationType>n:1</relationType>
                                        </foreignKey>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="KL.ORDNPRI"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                    <key name="for05_klass_ordnverdi_{$filnavn}">
                                        <foreignKey>
                                            <flatFileDefinitionReference name="ORDNVERDI">
                                                <recordDefinitionReferences>
                                                    <recordDefinitionReference name="ORDNVERDI">
                                                        <fieldDefinitionReferences>
                                                            <fieldDefinitionReference name="OV.ORDNPRI"/>
                                                            <fieldDefinitionReference name="OV.ORDNVER"/>
                                                        </fieldDefinitionReferences>
                                                    </recordDefinitionReference>
                                                </recordDefinitionReferences>
                                            </flatFileDefinitionReference>
                                            <relationType>n:1</relationType>
                                        </foreignKey>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="KL.ORDNPRI"/>
                                            <fieldDefinitionReference name="KL.ORDNVER"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                </keys>
                            </xsl:when>
                            <xsl:when test="$filnavn='SAKSPART'">
                                <keys>
                                    <key name="prim_{$filnavn}">
                                        <primaryKey/>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="SP.SAID"/>
                                            <fieldDefinitionReference name="SP.NAVN"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                </keys>
                            </xsl:when>
                            <xsl:when test="$filnavn='SAKSTATUS'">
                                <keys>
                                    <key name="prim_{$filnavn}">
                                        <primaryKey/>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="SS.STATUS"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                </keys>
                            </xsl:when>
                            <xsl:when test="$filnavn='SAKSTYPE'">
                                <keys>
                                    <key name="prim_{$filnavn}">
                                        <primaryKey/>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="ST.TYPE"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                </keys>
                            </xsl:when>
                            <xsl:when test="$filnavn='KASSKODE'">
                                <keys>
                                    <key name="prim_{$filnavn}">
                                        <primaryKey/>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="KK.KODE"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                </keys>
                            </xsl:when>
                            <xsl:when test="$filnavn='JOURNPOST'">
                                <keys>
                                    <key name="prim_{$filnavn}">
                                        <primaryKey/>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="JP.ID"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                    <key name="for06_noarksak_saar_{$filnavn}">
                                        <foreignKey>
                                            <flatFileDefinitionReference name="NOARKSAK">
                                                <recordDefinitionReferences>
                                                    <recordDefinitionReference name="NOARKSAK">
                                                        <fieldDefinitionReferences>
                                                            <fieldDefinitionReference name="SA.ID"/>
                                                        </fieldDefinitionReferences>
                                                    </recordDefinitionReference>
                                                </recordDefinitionReferences>
                                            </flatFileDefinitionReference>
                                            <relationType>n:1</relationType>
                                        </foreignKey>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="JP.SAID"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                    <key name="for07_pernavn_init_{$filnavn}">
                                        <foreignKey>
                                            <flatFileDefinitionReference name="PERNAVN">
                                                <recordDefinitionReferences>
                                                    <recordDefinitionReference name="PERNAVN">
                                                        <fieldDefinitionReferences>
                                                            <fieldDefinitionReference name="PN.ID"/>
                                                        </fieldDefinitionReferences>
                                                    </recordDefinitionReference>
                                                </recordDefinitionReferences>
                                            </flatFileDefinitionReference>
                                            <relationType>n:1</relationType>
                                        </foreignKey>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="JP.UTLTIL"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                </keys>
                            </xsl:when>
                            <xsl:when test="$filnavn='AVSMOT'">
                                <keys>
                                    <key name="prim_{$filnavn}">
                                        <primaryKey/>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="AM.ID"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                    <key name="for08_admindel_id_{$filnavn}">
                                        <foreignKey>
                                            <flatFileDefinitionReference name="ADMINDEL">
                                                <recordDefinitionReferences>
                                                    <recordDefinitionReference name="ADMINDEL">
                                                        <fieldDefinitionReferences>
                                                            <fieldDefinitionReference name="AI.ID"/>
                                                        </fieldDefinitionReferences>
                                                    </recordDefinitionReference>
                                                </recordDefinitionReferences>
                                            </flatFileDefinitionReference>
                                            <relationType>n:1</relationType>
                                        </foreignKey>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="AM.ADMID"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                    <key name="for09_pernavn_init_{$filnavn}">
                                        <foreignKey>
                                            <flatFileDefinitionReference name="PERNAVN">
                                                <recordDefinitionReferences>
                                                    <recordDefinitionReference name="PERNAVN">
                                                        <fieldDefinitionReferences>
                                                            <fieldDefinitionReference name="PN.ID"/>
                                                        </fieldDefinitionReferences>
                                                    </recordDefinitionReference>
                                                </recordDefinitionReferences>
                                            </flatFileDefinitionReference>
                                            <relationType>n:1</relationType>
                                        </foreignKey>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="AM.SBHID"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                    <key name="for10_journpst_avskriv_{$filnavn}">
                                        <foreignKey>
                                            <flatFileDefinitionReference name="JOURNPOST">
                                                <recordDefinitionReferences>
                                                    <recordDefinitionReference name="JOURNPOST">
                                                        <fieldDefinitionReferences>
                                                            <fieldDefinitionReference name="JP.ID"/>
                                                        </fieldDefinitionReferences>
                                                    </recordDefinitionReference>
                                                </recordDefinitionReferences>
                                            </flatFileDefinitionReference>
                                            <relationType>n:1</relationType>
                                        </foreignKey>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="AM.AVSKAV"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                    <key name="for11_journpst_besvar_{$filnavn}">
                                        <foreignKey>
                                            <flatFileDefinitionReference name="JOURNPOST">
                                                <recordDefinitionReferences>
                                                    <recordDefinitionReference name="JOURNPOST">
                                                        <fieldDefinitionReferences>
                                                            <fieldDefinitionReference name="JP.ID"/>
                                                        </fieldDefinitionReferences>
                                                    </recordDefinitionReference>
                                                </recordDefinitionReferences>
                                            </flatFileDefinitionReference>
                                            <relationType>n:1</relationType>
                                        </foreignKey>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="AM.BESVAR"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                </keys>
                            </xsl:when>
                            <xsl:when test="$filnavn='EKSTSAKREF'">
                                <keys>
                                    <key name="prim_{$filnavn}">
                                        <primaryKey/>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="ER.JPID"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                </keys>
                            </xsl:when>
                            <xsl:when test="$filnavn='NOARKDOKTYPE'">
                                <keys>
                                    <key name="prim_{$filnavn}">
                                        <primaryKey/>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="ND.DOKTYPE"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                </keys>
                            </xsl:when>
                            <xsl:when test="$filnavn='JOURNSTATUS'">
                                <keys>
                                    <key name="prim_{$filnavn}">
                                        <primaryKey/>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="JS.STATUS"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                </keys>
                            </xsl:when>
                            <xsl:when test="$filnavn='TLKODE'">
                                <keys>
                                    <key name="prim_{$filnavn}">
                                        <primaryKey/>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="TL.KODE"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                </keys>
                            </xsl:when>
                            <xsl:when test="$filnavn='AVGRADKODE'">
                                <keys>
                                    <key name="prim_{$filnavn}">
                                        <primaryKey/>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="AG.KODE"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                </keys>
                            </xsl:when>
                            <xsl:when test="$filnavn='AVSKRMAATE'">
                                <keys>
                                    <key name="prim_{$filnavn}">
                                        <primaryKey/>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="AV.KODE"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                </keys>
                            </xsl:when>
                            <xsl:when test="$filnavn='FORSMAATE'">
                                <keys>
                                    <key name="prim_{$filnavn}">
                                        <primaryKey/>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="FM.KODE"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                </keys>
                            </xsl:when>
                            <xsl:when test="$filnavn='FSTATUS'">
                                <keys>
                                    <key name="prim_{$filnavn}">
                                        <primaryKey/>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="FS.STATUS"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                </keys>
                            </xsl:when>
                            <xsl:when test="$filnavn='PRESEDENS'">
                                <keys>
                                    <key name="prim_{$filnavn}">
                                        <primaryKey/>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="PS.ID"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                    <key name="for12_admindel_admkort_{$filnavn}">
                                        <foreignKey>
                                            <flatFileDefinitionReference name="ADMINDEL">
                                                <recordDefinitionReferences>
                                                    <recordDefinitionReference name="ADMINDEL">
                                                        <fieldDefinitionReferences>
                                                            <fieldDefinitionReference name="AI.ID"/>
                                                        </fieldDefinitionReferences>
                                                    </recordDefinitionReference>
                                                </recordDefinitionReferences>
                                            </flatFileDefinitionReference>
                                            <relationType>n:1</relationType>
                                        </foreignKey>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="PS.ADMID"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                </keys>
                            </xsl:when>
                            <xsl:when test="$filnavn='PRESHENV'">
                                <keys>
                                    <key name="prim_{$filnavn}">
                                        <primaryKey/>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="PH.FPSID"/>
                                            <fieldDefinitionReference name="PH.TPSID"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                </keys>
                            </xsl:when>
                            <xsl:when test="$filnavn='EOPRESEDENS'">
                                <keys>
                                    <key name="prim_{$filnavn}">
                                        <primaryKey/>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="EP.PSID"/>
                                            <fieldDefinitionReference name="EP.ORD"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                </keys>
                            </xsl:when>
                            <xsl:when test="$filnavn='LFPRESEDENS'">
                                <keys>
                                    <key name="prim_{$filnavn}">
                                        <primaryKey/>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="PL.PSID"/>
                                            <fieldDefinitionReference name="PL.LOVFORSK"/>
                                            <fieldDefinitionReference name="PL.PARAGRAF"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                </keys>
                            </xsl:when>
                            <xsl:when test="$filnavn='LFKODER'">
                                <keys>
                                    <key name="prim_{$filnavn}">
                                        <primaryKey/>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="LK.KODE"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                </keys>
                            </xsl:when>
                            <xsl:when test="$filnavn='ADRESSEKP'">
                                <keys>
                                    <key name="prim_{$filnavn}">
                                        <primaryKey/>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="AK.ADRID"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                    <key name="alt_{$filnavn}">
                                        <alternateKey/>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="AK.KORTNAVN"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                </keys>
                            </xsl:when>
                            <xsl:when test="$filnavn='MEDLADRGR'">
                                <keys>
                                    <key name="prim_{$filnavn}">
                                        <primaryKey/>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="MG.GRID"/>
                                            <fieldDefinitionReference name="MG.MEDLID"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                </keys>
                            </xsl:when>
                            <xsl:when test="$filnavn='ADRTYPE'">
                                <keys>
                                    <key name="prim_{$filnavn}">
                                        <primaryKey/>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="AT.KODE"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                </keys>
                            </xsl:when>
                            <xsl:when test="$filnavn='POSTNR'">
                                <keys>
                                    <key name="prim_{$filnavn}">
                                        <primaryKey/>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="PO.POSTNR"/>
                                            <fieldDefinitionReference name="PO.NEDLDATO"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                    <key name="alt_{$filnavn}">
                                        <alternateKey/>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="PO.POSTNR"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                </keys>
                            </xsl:when>
                            <xsl:when test="$filnavn='KPAUTORIS'">
                                <keys>
                                    <key name="prim_{$filnavn}">
                                        <primaryKey/>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="KA.ADRID"/>
                                            <fieldDefinitionReference name="KA.TGKODE"/>
                                            <fieldDefinitionReference name="KA.TILDATO"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                    <key name="for13_pernavn_init_{$filnavn}">
                                        <foreignKey>
                                            <flatFileDefinitionReference name="PERNAVN">
                                                <recordDefinitionReferences>
                                                    <recordDefinitionReference name="PERNAVN">
                                                        <fieldDefinitionReferences>
                                                            <fieldDefinitionReference name="PN.ID"/>
                                                        </fieldDefinitionReferences>
                                                    </recordDefinitionReference>
                                                </recordDefinitionReferences>
                                            </flatFileDefinitionReference>
                                            <relationType>n:1</relationType>
                                        </foreignKey>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="KA.AUTAV"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                </keys>
                            </xsl:when>
                            <xsl:when test="$filnavn='DIGISERT'">
                                <keys>
                                    <key name="prim_{$filnavn}">
                                        <primaryKey/>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="SE.ID"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                    <key name="for14_pernavn_init_{$filnavn}">
                                        <foreignKey>
                                            <flatFileDefinitionReference name="PERNAVN">
                                                <recordDefinitionReferences>
                                                    <recordDefinitionReference name="PERNAVN">
                                                        <fieldDefinitionReferences>
                                                            <fieldDefinitionReference name="PN.ID"/>
                                                        </fieldDefinitionReferences>
                                                    </recordDefinitionReference>
                                                </recordDefinitionReferences>
                                            </flatFileDefinitionReference>
                                            <relationType>n:1</relationType>
                                        </foreignKey>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="SE.VERAV"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                </keys>
                            </xsl:when>
                            <xsl:when test="$filnavn='KPFORMAT'">
                                <keys>
                                    <key name="prim_{$filnavn}">
                                        <primaryKey/>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="KF.ADRID"/>
                                            <fieldDefinitionReference name="KF.FORMAT"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                </keys>
                            </xsl:when>
                            <xsl:when test="$filnavn='MERKNAD'">
                                <keys>
                                    <key name="prim_{$filnavn}">
                                        <primaryKey/>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="ME.ID"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                    <key name="for15_pernavn_regav_{$filnavn}">
                                        <foreignKey>
                                            <flatFileDefinitionReference name="PERNAVN">
                                                <recordDefinitionReferences>
                                                    <recordDefinitionReference name="PERNAVN">
                                                        <fieldDefinitionReferences>
                                                            <fieldDefinitionReference name="PN.ID"/>
                                                        </fieldDefinitionReferences>
                                                    </recordDefinitionReference>
                                                </recordDefinitionReferences>
                                            </flatFileDefinitionReference>
                                            <relationType>n:1</relationType>
                                        </foreignKey>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="ME.REGAV"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                    <key name="for16_pernavn_pvgav_{$filnavn}">
                                        <foreignKey>
                                            <flatFileDefinitionReference name="PERNAVN">
                                                <recordDefinitionReferences>
                                                    <recordDefinitionReference name="PERNAVN">
                                                        <fieldDefinitionReferences>
                                                            <fieldDefinitionReference name="PN.ID"/>
                                                        </fieldDefinitionReferences>
                                                    </recordDefinitionReference>
                                                </recordDefinitionReferences>
                                            </flatFileDefinitionReference>
                                            <relationType>n:1</relationType>
                                        </foreignKey>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="ME.PVGAV"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                </keys>
                            </xsl:when>
                            <xsl:when test="$filnavn='TILLEGGSINFO'">
                                <keys>
                                    <key name="prim_{$filnavn}">
                                        <primaryKey/>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="TI.ID"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                    <key name="for17_pernavn_regav_{$filnavn}">
                                        <foreignKey>
                                            <flatFileDefinitionReference name="PERNAVN">
                                                <recordDefinitionReferences>
                                                    <recordDefinitionReference name="PERNAVN">
                                                        <fieldDefinitionReferences>
                                                            <fieldDefinitionReference name="PN.ID"/>
                                                        </fieldDefinitionReferences>
                                                    </recordDefinitionReference>
                                                </recordDefinitionReferences>
                                            </flatFileDefinitionReference>
                                            <relationType>n:1</relationType>
                                        </foreignKey>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="TI.REGAV"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                    <key name="for18_pernavn_pvgav_{$filnavn}">
                                        <foreignKey>
                                            <flatFileDefinitionReference name="PERNAVN">
                                                <recordDefinitionReferences>
                                                    <recordDefinitionReference name="PERNAVN">
                                                        <fieldDefinitionReferences>
                                                            <fieldDefinitionReference name="PN.ID"/>
                                                        </fieldDefinitionReferences>
                                                    </recordDefinitionReference>
                                                </recordDefinitionReferences>
                                            </flatFileDefinitionReference>
                                            <relationType>n:1</relationType>
                                        </foreignKey>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="TI.PVGAV"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                </keys>
                            </xsl:when>
                            <xsl:when test="$filnavn='INFOTYPE'">
                                <keys>
                                    <key name="prim_{$filnavn}">
                                        <primaryKey/>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="IT.KODE"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                </keys>
                            </xsl:when>
                            <xsl:when test="$filnavn='DOKLINK'">
                                <keys>
                                    <key name="prim_{$filnavn}">
                                        <primaryKey/>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="DL.JPID"/>
                                            <fieldDefinitionReference name="DL.RNR"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                    <key name="for19_pernavn_tkav_{$filnavn}">
                                        <foreignKey>
                                            <flatFileDefinitionReference name="PERNAVN">
                                                <recordDefinitionReferences>
                                                    <recordDefinitionReference name="PERNAVN">
                                                        <fieldDefinitionReferences>
                                                            <fieldDefinitionReference name="PN.ID"/>
                                                        </fieldDefinitionReferences>
                                                    </recordDefinitionReference>
                                                </recordDefinitionReferences>
                                            </flatFileDefinitionReference>
                                            <relationType>n:1</relationType>
                                        </foreignKey>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="DL.TKAV"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                </keys>
                            </xsl:when>
                            <xsl:when test="$filnavn='DOKBESKRIV'">
                                <keys>
                                    <key name="prim_{$filnavn}">
                                        <primaryKey/>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="DB.DOKID"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                    <key name="for20_pernavn_utarbav_{$filnavn}">
                                        <foreignKey>
                                            <flatFileDefinitionReference name="PERNAVN">
                                                <recordDefinitionReferences>
                                                    <recordDefinitionReference name="PERNAVN">
                                                        <fieldDefinitionReferences>
                                                            <fieldDefinitionReference name="PN.ID"/>
                                                        </fieldDefinitionReferences>
                                                    </recordDefinitionReference>
                                                </recordDefinitionReferences>
                                            </flatFileDefinitionReference>
                                            <relationType>n:1</relationType>
                                        </foreignKey>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="DB.UTARBAV"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                </keys>
                            </xsl:when>
                            <xsl:when test="$filnavn='DOKVERSJON'">
                                <keys>
                                    <key name="prim_{$filnavn}">
                                        <primaryKey/>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="VE.DOKID"/>
                                            <fieldDefinitionReference name="VE.VERSJON"/>
                                            <fieldDefinitionReference name="VE.VARIANT"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                    <key name="for21_pernavn_regav_{$filnavn}">
                                        <foreignKey>
                                            <flatFileDefinitionReference name="PERNAVN">
                                                <recordDefinitionReferences>
                                                    <recordDefinitionReference name="PERNAVN">
                                                        <fieldDefinitionReferences>
                                                            <fieldDefinitionReference name="PN.ID"/>
                                                        </fieldDefinitionReferences>
                                                    </recordDefinitionReference>
                                                </recordDefinitionReferences>
                                            </flatFileDefinitionReference>
                                            <relationType>n:1</relationType>
                                        </foreignKey>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="VE.REGAV"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                </keys>
                            </xsl:when>
                            <xsl:when test="$filnavn='DIGISIGN'">
                                <keys>
                                    <key name="prim_{$filnavn}">
                                        <primaryKey/>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="DI.DOKID"/>
                                            <fieldDefinitionReference name="DI.VERSJON"/>
                                            <fieldDefinitionReference name="DI.VARIANT"/>
                                            <fieldDefinitionReference name="DI.SIGNNR"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                    <key name="for22_pernavn_sigverav_{$filnavn}">
                                        <foreignKey>
                                            <flatFileDefinitionReference name="PERNAVN">
                                                <recordDefinitionReferences>
                                                    <recordDefinitionReference name="PERNAVN">
                                                        <fieldDefinitionReferences>
                                                            <fieldDefinitionReference name="PN.ID"/>
                                                        </fieldDefinitionReferences>
                                                    </recordDefinitionReference>
                                                </recordDefinitionReferences>
                                            </flatFileDefinitionReference>
                                            <relationType>n:1</relationType>
                                        </foreignKey>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="DI.SIGVERAV"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                </keys>
                            </xsl:when>
                            <xsl:when test="$filnavn='EODOKUMENT'">
                                <keys>
                                    <key name="prim_{$filnavn}">
                                        <primaryKey/>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="ED.DOKID"/>
                                            <fieldDefinitionReference name="ED.ORD"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                </keys>
                            </xsl:when>
                            <xsl:when test="$filnavn='EMNEORD'">
                                <keys>
                                    <key name="prim_{$filnavn}">
                                        <primaryKey/>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="EO.EMNEORD"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                </keys>
                            </xsl:when>
                            <xsl:when test="$filnavn='EOHENV'">
                                <keys>
                                    <key name="prim_{$filnavn}">
                                        <primaryKey/>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="ES.EMNEORD1"/>
                                            <fieldDefinitionReference name="ES.EMNEORD2"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                </keys>
                            </xsl:when>
                            <xsl:when test="$filnavn='EOHIERARKI'">
                                <keys>
                                    <key name="prim_{$filnavn}">
                                        <primaryKey/>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="EH.EMNEORD1"/>
                                            <fieldDefinitionReference name="EH.EMNEORD2"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                </keys>
                            </xsl:when>
                            <xsl:when test="$filnavn='DOKTILKN'">
                                <keys>
                                    <key name="prim_{$filnavn}">
                                        <primaryKey/>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="DT.KODE"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                </keys>
                            </xsl:when>
                            <xsl:when test="$filnavn='DOKKATEGORI'">
                                <keys>
                                    <key name="prim_{$filnavn}">
                                        <primaryKey/>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="DK.KODE"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                </keys>
                            </xsl:when>
                            <xsl:when test="$filnavn='DOKSTATUS'">
                                <keys>
                                    <key name="prim_{$filnavn}">
                                        <primaryKey/>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="DS.STATUS"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                </keys>
                            </xsl:when>
                            <xsl:when test="$filnavn='VARIANTFORMAT'">
                                <keys>
                                    <key name="prim_{$filnavn}">
                                        <primaryKey/>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="VF.KODE"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                </keys>
                            </xsl:when>
                            <xsl:when test="$filnavn='LAGRFORMAT'">
                                <keys>
                                    <key name="prim_{$filnavn}">
                                        <primaryKey/>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="LF.KODE"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                </keys>
                            </xsl:when>
                            <xsl:when test="$filnavn='LAGRENHET'">
                                <keys>
                                    <key name="prim_{$filnavn}">
                                        <primaryKey/>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="LA.KODE"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                </keys>
                            </xsl:when>
                            <xsl:when test="$filnavn='ADMINDEL'">
                                <keys>
                                    <key name="prim_{$filnavn}">
                                        <primaryKey/>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="AI.ID"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                    <key name="for23_admindel_alidfar_{$filnavn}">
                                        <foreignKey>
                                            <flatFileDefinitionReference name="ADMINDEL">
                                                <recordDefinitionReferences>
                                                    <recordDefinitionReference name="ADMINDEL">
                                                        <fieldDefinitionReferences>
                                                            <fieldDefinitionReference name="AI.ID"/>
                                                        </fieldDefinitionReferences>
                                                    </recordDefinitionReference>
                                                </recordDefinitionReferences>
                                            </flatFileDefinitionReference>
                                            <relationType>n:1</relationType>
                                        </foreignKey>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="AI.IDFAR"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                </keys>
                            </xsl:when>
                            <xsl:when test="$filnavn='ALIASADMENH'">
                                <keys>
                                    <key name="prim_{$filnavn}">
                                        <primaryKey/>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="AL.ADMIDFRA"/>
                                            <fieldDefinitionReference name="AL.ADMIDTIL"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                </keys>
                            </xsl:when>
                            <xsl:when test="$filnavn='ADRADMENH'">
                                <keys>
                                    <key name="prim_{$filnavn}">
                                        <primaryKey/>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="AA.ADMID"/>
                                            <fieldDefinitionReference name="AA.ADRID"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                </keys>
                            </xsl:when>
                            <xsl:when test="$filnavn='ENHETSTYPE'">
                                <keys>
                                    <key name="prim_{$filnavn}">
                                        <primaryKey/>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="ET.KODE"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                </keys>
                            </xsl:when>
                            <xsl:when test="$filnavn='ARKIVPERIODE'">
                                <keys>
                                    <key name="prim_{$filnavn}">
                                        <primaryKey/>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="AP.PERIODE"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                </keys>
                            </xsl:when>
                            <xsl:when test="$filnavn='ARKIV'">
                                <keys>
                                    <key name="prim_{$filnavn}">
                                        <primaryKey/>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="AR.ARKIV"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                </keys>
                            </xsl:when>
                            <xsl:when test="$filnavn='ARKIVDEL'">
                                <keys>
                                    <key name="prim_{$filnavn}">
                                        <primaryKey/>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="AD.ARKDEL"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                    <key name="for24_pernavn_kontrav_{$filnavn}">
                                        <foreignKey>
                                            <flatFileDefinitionReference name="PERNAVN">
                                                <recordDefinitionReferences>
                                                    <recordDefinitionReference name="PERNAVN">
                                                        <fieldDefinitionReferences>
                                                            <fieldDefinitionReference name="PN.ID"/>
                                                        </fieldDefinitionReferences>
                                                    </recordDefinitionReference>
                                                </recordDefinitionReferences>
                                            </flatFileDefinitionReference>
                                            <relationType>n:1</relationType>
                                        </foreignKey>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="AD.KONTRAV"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                </keys>
                            </xsl:when>
                            <xsl:when test="$filnavn='ORDNPRINS'">
                                <keys>
                                    <key name="prim_{$filnavn}">
                                        <primaryKey/>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="OP.ORDNPRI"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                </keys>
                            </xsl:when>
                            <xsl:when test="$filnavn='ORDNVERDI'">
                                <keys>
                                    <key name="prim_{$filnavn}">
                                        <primaryKey/>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="OV.ORDNPRI"/>
                                            <fieldDefinitionReference name="OV.ORDNVER"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                    <key name="for25_ordnpri_ordnpri_{$filnavn}">
                                        <foreignKey>
                                            <flatFileDefinitionReference name="ORDNPRINS">
                                                <recordDefinitionReferences>
                                                    <recordDefinitionReference name="ORDNPRINS">
                                                        <fieldDefinitionReferences>
                                                            <fieldDefinitionReference name="OP.ORDNPRI"/>
                                                        </fieldDefinitionReferences>
                                                    </recordDefinitionReference>
                                                </recordDefinitionReferences>
                                            </flatFileDefinitionReference>
                                            <relationType>n:1</relationType>
                                        </foreignKey>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="OV.ORDNPRI"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                </keys>
                            </xsl:when>
                            <xsl:when test="$filnavn='JFORDNVER'">
                                <keys>
                                    <key name="prim_{$filnavn}">
                                        <primaryKey/>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="JO.ORDNPRI1"/>
                                            <fieldDefinitionReference name="JO.ORDNVER1"/>
                                            <fieldDefinitionReference name="JO.ORDNPRI2"/>
                                            <fieldDefinitionReference name="JO.ORDNVER2"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                </keys>
                            </xsl:when>
                            <xsl:when test="$filnavn='EARKKODE'">
                                <keys>
                                    <key name="prim_{$filnavn}">
                                        <primaryKey/>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="EA.ORDNPRI"/>
                                            <fieldDefinitionReference name="EA.ORDNVER"/>
                                            <fieldDefinitionReference name="EA.ORD"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                </keys>
                            </xsl:when>
                            <xsl:when test="$filnavn='JOURNENHET'">
                                <keys>
                                    <key name="prim_{$filnavn}">
                                        <primaryKey/>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="JE.JENHET"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                </keys>
                            </xsl:when>
                            <xsl:when test="$filnavn='NUMSERIE'">
                                <keys>
                                    <key name="prim_{$filnavn}">
                                        <primaryKey/>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="NU.ID"/>
                                            <fieldDefinitionReference name="NU.AAR"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                </keys>
                            </xsl:when>
                            <xsl:when test="$filnavn='JENARKDEL'">
                                <keys>
                                    <key name="prim_{$filnavn}">
                                        <primaryKey/>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="JA.JENHET"/>
                                            <fieldDefinitionReference name="JA.ARKDEL"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                </keys>
                            </xsl:when>
                            <xsl:when test="$filnavn='ARSTATUS'">
                                <keys>
                                    <key name="prim_{$filnavn}">
                                        <primaryKey/>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="AS.STATUS"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                </keys>
                            </xsl:when>
                            <xsl:when test="$filnavn='BSKODE'">
                                <keys>
                                    <key name="prim_{$filnavn}">
                                        <primaryKey/>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="BK.KODE"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                </keys>
                            </xsl:when>
                            <xsl:when test="$filnavn='ORDNPRINSTYPE'">
                                <keys>
                                    <key name="prim_{$filnavn}">
                                        <primaryKey/>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="OT.KODE"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                </keys>
                            </xsl:when>
                            <xsl:when test="$filnavn='PERSON'">
                                <keys>
                                    <key name="prim_{$filnavn}">
                                        <primaryKey/>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="PE.ID"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                    <key name="for26_pernavn_peid_{$filnavn}">
                                        <foreignKey>
                                            <flatFileDefinitionReference name="PERNAVN">
                                                <recordDefinitionReferences>
                                                    <recordDefinitionReference name="PERNAVN">
                                                        <fieldDefinitionReferences>
                                                            <fieldDefinitionReference name="PN.ID"/>
                                                        </fieldDefinitionReferences>
                                                    </recordDefinitionReference>
                                                </recordDefinitionReferences>
                                            </flatFileDefinitionReference>
                                            <relationType>n:1</relationType>
                                        </foreignKey>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="PE.PEID"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                </keys>
                            </xsl:when>
                            <xsl:when test="$filnavn='PERNAVN'">
                                <keys>
                                    <key name="prim_{$filnavn}">
                                        <primaryKey/>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="PN.ID"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                </keys>
                            </xsl:when>
                            <xsl:when test="$filnavn='ADRPERSON'">
                                <keys>
                                    <key name="prim_{$filnavn}">
                                        <primaryKey/>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="PA.PEID"/>
                                            <fieldDefinitionReference name="PA.ADRID"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                </keys>
                            </xsl:when>
                            <xsl:when test="$filnavn='PERROLLE'">
                                <keys>
                                    <key name="prim_{$filnavn}">
                                        <primaryKey/>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="PR.ID"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                    <key name="for27_pernavn_PEID_{$filnavn}">
                                        <foreignKey>
                                            <flatFileDefinitionReference name="PERNAVN">
                                                <recordDefinitionReferences>
                                                    <recordDefinitionReference name="PERNAVN">
                                                        <fieldDefinitionReferences>
                                                            <fieldDefinitionReference name="PN.PEID"/>
                                                        </fieldDefinitionReferences>
                                                    </recordDefinitionReference>
                                                </recordDefinitionReferences>
                                            </flatFileDefinitionReference>
                                            <relationType>n:1</relationType>
                                        </foreignKey>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="PR.PEID"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                </keys>
                            </xsl:when>
                            <xsl:when test="$filnavn='FUNGROLLE'">
                                <keys>
                                    <key name="prim_{$filnavn}">
                                        <primaryKey/>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="FR.PRID"/>
                                            <fieldDefinitionReference name="FR.FUNGPRID"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                </keys>
                            </xsl:when>
                            <xsl:when test="$filnavn='ROLLE'">
                                <keys>
                                    <key name="prim_{$filnavn}">
                                        <primaryKey/>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="RO.ROLLEID"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                </keys>
                            </xsl:when>
                            <xsl:when test="$filnavn='TGINFO'">
                                <keys>
                                    <key name="prim_{$filnavn}">
                                        <primaryKey/>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="TJ.PEID"/>
                                            <fieldDefinitionReference name="TJ.JENHET"/>
                                            <fieldDefinitionReference name="TJ.ADMID"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                    <key name="for28_pernavn_PEID_{$filnavn}">
                                        <foreignKey>
                                            <flatFileDefinitionReference name="PERNAVN">
                                                <recordDefinitionReferences>
                                                    <recordDefinitionReference name="PERNAVN">
                                                        <fieldDefinitionReferences>
                                                            <fieldDefinitionReference name="PN.PEID"/>
                                                        </fieldDefinitionReferences>
                                                    </recordDefinitionReference>
                                                </recordDefinitionReferences>
                                            </flatFileDefinitionReference>
                                            <relationType>n:1</relationType>
                                        </foreignKey>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="TJ.PEID"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                    <key name="for29_admindel_ADMID_{$filnavn}">
                                        <foreignKey>
                                            <flatFileDefinitionReference name="ADMINDEL">
                                                <recordDefinitionReferences>
                                                    <recordDefinitionReference name="ADMINDEL">
                                                        <fieldDefinitionReferences>
                                                            <fieldDefinitionReference name="AI.ID"/>
                                                        </fieldDefinitionReferences>
                                                    </recordDefinitionReference>
                                                </recordDefinitionReferences>
                                            </flatFileDefinitionReference>
                                            <relationType>n:1</relationType>
                                        </foreignKey>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="TJ.ADMID"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                    <key name="for30_pernavn_autav_{$filnavn}">
                                        <foreignKey>
                                            <flatFileDefinitionReference name="PERNAVN">
                                                <recordDefinitionReferences>
                                                    <recordDefinitionReference name="PERNAVN">
                                                        <fieldDefinitionReferences>
                                                            <fieldDefinitionReference name="PN.PEID"/>
                                                        </fieldDefinitionReferences>
                                                    </recordDefinitionReference>
                                                </recordDefinitionReferences>
                                            </flatFileDefinitionReference>
                                            <relationType>n:1</relationType>
                                        </foreignKey>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="TJ.AUTAV"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                    <key name="for31_pernavn_autoppav_{$filnavn}">
                                        <foreignKey>
                                            <flatFileDefinitionReference name="PERNAVN">
                                                <recordDefinitionReferences>
                                                    <recordDefinitionReference name="PERNAVN">
                                                        <fieldDefinitionReferences>
                                                            <fieldDefinitionReference name="PN.PEID"/>
                                                        </fieldDefinitionReferences>
                                                    </recordDefinitionReference>
                                                </recordDefinitionReferences>
                                            </flatFileDefinitionReference>
                                            <relationType>n:1</relationType>
                                        </foreignKey>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="TJ.AUTOPPAV"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                </keys>
                            </xsl:when>
                            <xsl:when test="$filnavn='TGKODE'">
                                <keys>
                                    <key name="prim_{$filnavn}">
                                        <primaryKey/>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="TK.TGKODE"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                </keys>
                            </xsl:when>
                            <xsl:when test="$filnavn='TGHJEMMEL'">
                                <keys>
                                    <key name="prim_{$filnavn}">
                                        <primaryKey/>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="TH.TGKODE"/>
                                            <fieldDefinitionReference name="TH.UOFF"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                </keys>
                            </xsl:when>
                            <xsl:when test="$filnavn='PERKLARER'">
                                <keys>
                                    <key name="prim_{$filnavn}">
                                        <primaryKey/>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="KT.PEID"/>
                                            <fieldDefinitionReference name="KT.TGKODE"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                    <key name="for32_pernavn_peid_{$filnavn}">
                                        <foreignKey>
                                            <flatFileDefinitionReference name="PERNAVN">
                                                <recordDefinitionReferences>
                                                    <recordDefinitionReference name="PERNAVN">
                                                        <fieldDefinitionReferences>
                                                            <fieldDefinitionReference name="PN.PEID"/>
                                                        </fieldDefinitionReferences>
                                                    </recordDefinitionReference>
                                                </recordDefinitionReferences>
                                            </flatFileDefinitionReference>
                                            <relationType>n:1</relationType>
                                        </foreignKey>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="KT.PEID"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                    <key name="for33_pernavn_klav_{$filnavn}">
                                        <foreignKey>
                                            <flatFileDefinitionReference name="PERNAVN">
                                                <recordDefinitionReferences>
                                                    <recordDefinitionReference name="PERNAVN">
                                                        <fieldDefinitionReferences>
                                                            <fieldDefinitionReference name="PN.PEID"/>
                                                        </fieldDefinitionReferences>
                                                    </recordDefinitionReference>
                                                </recordDefinitionReferences>
                                            </flatFileDefinitionReference>
                                            <relationType>n:1</relationType>
                                        </foreignKey>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="KT.KLAV"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                    <key name="for34_pernavn_kloppav_{$filnavn}">
                                        <foreignKey>
                                            <flatFileDefinitionReference name="PERNAVN">
                                                <recordDefinitionReferences>
                                                    <recordDefinitionReference name="PERNAVN">
                                                        <fieldDefinitionReferences>
                                                            <fieldDefinitionReference name="PN.PEID"/>
                                                        </fieldDefinitionReferences>
                                                    </recordDefinitionReference>
                                                </recordDefinitionReferences>
                                            </flatFileDefinitionReference>
                                            <relationType>n:1</relationType>
                                        </foreignKey>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="KT.KLOPPAV"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                </keys>
                            </xsl:when>
                            <xsl:when test="$filnavn='PERTGKODE'">
                                <keys>
                                    <key name="prim_{$filnavn}">
                                        <primaryKey/>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="PT.PEID"/>
                                            <fieldDefinitionReference name="PT.TGKODE"/>
                                            <fieldDefinitionReference name="PT.ADMID"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                    <key name="for35_pernavn_peid_{$filnavn}">
                                        <foreignKey>
                                            <flatFileDefinitionReference name="PERNAVN">
                                                <recordDefinitionReferences>
                                                    <recordDefinitionReference name="PERNAVN">
                                                        <fieldDefinitionReferences>
                                                            <fieldDefinitionReference name="PN.PEID"/>
                                                        </fieldDefinitionReferences>
                                                    </recordDefinitionReference>
                                                </recordDefinitionReferences>
                                            </flatFileDefinitionReference>
                                            <relationType>n:1</relationType>
                                        </foreignKey>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="PT.PEID"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                    <key name="for36_admindel_admid_{$filnavn}">
                                        <foreignKey>
                                            <flatFileDefinitionReference name="ADMINDEL">
                                                <recordDefinitionReferences>
                                                    <recordDefinitionReference name="ADMINDEL">
                                                        <fieldDefinitionReferences>
                                                            <fieldDefinitionReference name="AI.ID"/>
                                                        </fieldDefinitionReferences>
                                                    </recordDefinitionReference>
                                                </recordDefinitionReferences>
                                            </flatFileDefinitionReference>
                                            <relationType>n:1</relationType>
                                        </foreignKey>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="PT.ADMID"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                    <key name="for37_pernavn_autav_{$filnavn}">
                                        <foreignKey>
                                            <flatFileDefinitionReference name="PERNAVN">
                                                <recordDefinitionReferences>
                                                    <recordDefinitionReference name="PERNAVN">
                                                        <fieldDefinitionReferences>
                                                            <fieldDefinitionReference name="PN.PEID"/>
                                                        </fieldDefinitionReferences>
                                                    </recordDefinitionReference>
                                                </recordDefinitionReferences>
                                            </flatFileDefinitionReference>
                                            <relationType>n:1</relationType>
                                        </foreignKey>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="PT.AUTAV"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                    <key name="for38_pernavn_autoppav_{$filnavn}">
                                        <foreignKey>
                                            <flatFileDefinitionReference name="PERNAVN">
                                                <recordDefinitionReferences>
                                                    <recordDefinitionReference name="PERNAVN">
                                                        <fieldDefinitionReferences>
                                                            <fieldDefinitionReference name="PN.PEID"/>
                                                        </fieldDefinitionReferences>
                                                    </recordDefinitionReference>
                                                </recordDefinitionReferences>
                                            </flatFileDefinitionReference>
                                            <relationType>n:1</relationType>
                                        </foreignKey>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="PT.AUTOPPAV"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                </keys>
                            </xsl:when>
                            <xsl:when test="$filnavn='TGGRUPPE'">
                                <keys>
                                    <key name="prim_{$filnavn}">
                                        <primaryKey/>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="TG.GRUPPEID"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                    <key name="for39_pernavn_oppav_{$filnavn}">
                                        <foreignKey>
                                            <flatFileDefinitionReference name="PERNAVN">
                                                <recordDefinitionReferences>
                                                    <recordDefinitionReference name="PERNAVN">
                                                        <fieldDefinitionReferences>
                                                            <fieldDefinitionReference name="PN.PEID"/>
                                                        </fieldDefinitionReferences>
                                                    </recordDefinitionReference>
                                                </recordDefinitionReferences>
                                            </flatFileDefinitionReference>
                                            <relationType>n:1</relationType>
                                        </foreignKey>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="TG.OPPAV"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                </keys>
                            </xsl:when>
                            <xsl:when test="$filnavn='TGMEDLEM'">
                                <keys>
                                    <key name="prim_{$filnavn}">
                                        <primaryKey/>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="PG.PERROLID"/>
                                            <fieldDefinitionReference name="PG.GRUPPEID"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                    <key name="for40_pernavn_peid_{$filnavn}">
                                        <foreignKey>
                                            <flatFileDefinitionReference name="PERNAVN">
                                                <recordDefinitionReferences>
                                                    <recordDefinitionReference name="PERNAVN">
                                                        <fieldDefinitionReferences>
                                                            <fieldDefinitionReference name="PN.PEID"/>
                                                        </fieldDefinitionReferences>
                                                    </recordDefinitionReference>
                                                </recordDefinitionReferences>
                                            </flatFileDefinitionReference>
                                            <relationType>n:1</relationType>
                                        </foreignKey>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="PG.PEID"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                    <key name="for41_pernavn_innmav_{$filnavn}">
                                        <foreignKey>
                                            <flatFileDefinitionReference name="PERNAVN">
                                                <recordDefinitionReferences>
                                                    <recordDefinitionReference name="PERNAVN">
                                                        <fieldDefinitionReferences>
                                                            <fieldDefinitionReference name="PN.PEID"/>
                                                        </fieldDefinitionReferences>
                                                    </recordDefinitionReference>
                                                </recordDefinitionReferences>
                                            </flatFileDefinitionReference>
                                            <relationType>n:1</relationType>
                                        </foreignKey>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="PG.INNMAV"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                    <key name="for42_pernavn_utmav_{$filnavn}">
                                        <foreignKey>
                                            <flatFileDefinitionReference name="PERNAVN">
                                                <recordDefinitionReferences>
                                                    <recordDefinitionReference name="PERNAVN">
                                                        <fieldDefinitionReferences>
                                                            <fieldDefinitionReference name="PN.PEID"/>
                                                        </fieldDefinitionReferences>
                                                    </recordDefinitionReference>
                                                </recordDefinitionReferences>
                                            </flatFileDefinitionReference>
                                            <relationType>n:1</relationType>
                                        </foreignKey>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="PG.UTMAV"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                </keys>
                            </xsl:when>
                            <xsl:when test="$filnavn='UTVALG'">
                                <keys>
                                    <key name="prim_{$filnavn}">
                                        <primaryKey/>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="UT.ID"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                    <key name="for43_admindel_admid_{$filnavn}">
                                        <foreignKey>
                                            <flatFileDefinitionReference name="ADMINDEL">
                                                <recordDefinitionReferences>
                                                    <recordDefinitionReference name="ADMINDEL">
                                                        <fieldDefinitionReferences>
                                                            <fieldDefinitionReference name="AI.ID"/>
                                                        </fieldDefinitionReferences>
                                                    </recordDefinitionReference>
                                                </recordDefinitionReferences>
                                            </flatFileDefinitionReference>
                                            <relationType>n:1</relationType>
                                        </foreignKey>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="UT.ADMID"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                    <key name="for44_adressek_oppnav_{$filnavn}">
                                        <foreignKey>
                                            <flatFileDefinitionReference name="ADRESSEKP">
                                                <recordDefinitionReferences>
                                                    <recordDefinitionReference name="ADRESSEKP">
                                                        <fieldDefinitionReferences>
                                                            <fieldDefinitionReference name="AK.ADRID"/>
                                                        </fieldDefinitionReferences>
                                                    </recordDefinitionReference>
                                                </recordDefinitionReferences>
                                            </flatFileDefinitionReference>
                                            <relationType>n:1</relationType>
                                        </foreignKey>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="UT.OPPNAV"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                    <key name="for45_utvalg_oppnavutv_{$filnavn}">
                                        <foreignKey>
                                            <flatFileDefinitionReference name="UTVALG">
                                                <recordDefinitionReferences>
                                                    <recordDefinitionReference name="UTVALG">
                                                        <fieldDefinitionReferences>
                                                            <fieldDefinitionReference name="UT.ID"/>
                                                        </fieldDefinitionReferences>
                                                    </recordDefinitionReference>
                                                </recordDefinitionReferences>
                                            </flatFileDefinitionReference>
                                            <relationType>n:1</relationType>
                                        </foreignKey>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="UT.OPPNAVUTV"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                </keys>
                            </xsl:when>
                            <xsl:when test="$filnavn='UTVMOTE'">
                                <keys>
                                    <key name="prim_{$filnavn}">
                                        <primaryKey/>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="MO.ID"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                    <key name="for46_utvalg_utvid_{$filnavn}">
                                        <foreignKey>
                                            <flatFileDefinitionReference name="UTVALG">
                                                <recordDefinitionReferences>
                                                    <recordDefinitionReference name="UTVALG">
                                                        <fieldDefinitionReferences>
                                                            <fieldDefinitionReference name="UT.ID"/>
                                                        </fieldDefinitionReferences>
                                                    </recordDefinitionReference>
                                                </recordDefinitionReferences>
                                            </flatFileDefinitionReference>
                                            <relationType>n:1</relationType>
                                        </foreignKey>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="MO.UTVID"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                </keys>
                            </xsl:when>
                            <xsl:when test="$filnavn='UTVMEDLEM'">
                                <keys>
                                    <key name="prim_{$filnavn}">
                                        <primaryKey/>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="UM.UTVKODE"/>
                                            <fieldDefinitionReference name="UM.PNID"/>
                                            <fieldDefinitionReference name="UM.TILDATO"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                    <key name="for47_utvalg_utvid_{$filnavn}">
                                        <foreignKey>
                                            <flatFileDefinitionReference name="UTVALG">
                                                <recordDefinitionReferences>
                                                    <recordDefinitionReference name="UTVALG">
                                                        <fieldDefinitionReferences>
                                                            <fieldDefinitionReference name="UT.ID"/>
                                                        </fieldDefinitionReferences>
                                                    </recordDefinitionReference>
                                                </recordDefinitionReferences>
                                            </flatFileDefinitionReference>
                                            <relationType>n:1</relationType>
                                        </foreignKey>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="UM.UTVID"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                    <key name="for48_pernavn_pnid_{$filnavn}">
                                        <foreignKey>
                                            <flatFileDefinitionReference name="PERNAVN">
                                                <recordDefinitionReferences>
                                                    <recordDefinitionReference name="PERNAVN">
                                                        <fieldDefinitionReferences>
                                                            <fieldDefinitionReference name="PN.ID"/>
                                                        </fieldDefinitionReferences>
                                                    </recordDefinitionReference>
                                                </recordDefinitionReferences>
                                            </flatFileDefinitionReference>
                                            <relationType>n:1</relationType>
                                        </foreignKey>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="UM.PNID"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                    <key name="for49_pernavn_varaforid_{$filnavn}">
                                        <foreignKey>
                                            <flatFileDefinitionReference name="PERNAVN">
                                                <recordDefinitionReferences>
                                                    <recordDefinitionReference name="PERNAVN">
                                                        <fieldDefinitionReferences>
                                                            <fieldDefinitionReference name="PN.ID"/>
                                                        </fieldDefinitionReferences>
                                                    </recordDefinitionReference>
                                                </recordDefinitionReferences>
                                            </flatFileDefinitionReference>
                                            <relationType>n:1</relationType>
                                        </foreignKey>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="UM.VARAFORID"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                    <key name="for50_adressek_repres_{$filnavn}">
                                        <foreignKey>
                                            <flatFileDefinitionReference name="ADRESSEKP">
                                                <recordDefinitionReferences>
                                                    <recordDefinitionReference name="ADRESSEKP">
                                                        <fieldDefinitionReferences>
                                                            <fieldDefinitionReference name="AK.ADRID"/>
                                                        </fieldDefinitionReferences>
                                                    </recordDefinitionReference>
                                                </recordDefinitionReferences>
                                            </flatFileDefinitionReference>
                                            <relationType>n:1</relationType>
                                        </foreignKey>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="UM.REPRES"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                </keys>
                            </xsl:when>
                            <xsl:when test="$filnavn='FRAMMOTE'">
                                <keys>
                                    <key name="prim_{$filnavn}">
                                        <primaryKey/>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="FU.MOID"/>
                                            <fieldDefinitionReference name="FU.PNID"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                    <key name="for51_pernavn_pnid_{$filnavn}">
                                        <foreignKey>
                                            <flatFileDefinitionReference name="PERNAVN">
                                                <recordDefinitionReferences>
                                                    <recordDefinitionReference name="PERNAVN">
                                                        <fieldDefinitionReferences>
                                                            <fieldDefinitionReference name="PN.ID"/>
                                                        </fieldDefinitionReferences>
                                                    </recordDefinitionReference>
                                                </recordDefinitionReferences>
                                            </flatFileDefinitionReference>
                                            <relationType>n:1</relationType>
                                        </foreignKey>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="FU.PNID"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                    <key name="for52_pernavn_varaforid_{$filnavn}">
                                        <foreignKey>
                                            <flatFileDefinitionReference name="PERNAVN">
                                                <recordDefinitionReferences>
                                                    <recordDefinitionReference name="PERNAVN">
                                                        <fieldDefinitionReferences>
                                                            <fieldDefinitionReference name="PN.ID"/>
                                                        </fieldDefinitionReferences>
                                                    </recordDefinitionReference>
                                                </recordDefinitionReferences>
                                            </flatFileDefinitionReference>
                                            <relationType>n:1</relationType>
                                        </foreignKey>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="FU.VARAFORID"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                    <key name="for53_utvmote_moid_{$filnavn}">
                                        <foreignKey>
                                            <flatFileDefinitionReference name="UTVMOTE">
                                                <recordDefinitionReferences>
                                                    <recordDefinitionReference name="UTVMOTE">
                                                        <fieldDefinitionReferences>
                                                            <fieldDefinitionReference name="MO.ID"/>
                                                        </fieldDefinitionReferences>
                                                    </recordDefinitionReference>
                                                </recordDefinitionReferences>
                                            </flatFileDefinitionReference>
                                            <relationType>n:1</relationType>
                                        </foreignKey>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="FU.MOID"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                </keys>
                            </xsl:when>
                            <xsl:when test="$filnavn='UTVMOTEDOK'">
                                <keys>
                                    <key name="prim_{$filnavn}">
                                        <primaryKey/>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="MD.ID"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                    <key name="for54_utvalg_utvid_{$filnavn}">
                                        <foreignKey>
                                            <flatFileDefinitionReference name="UTVALG">
                                                <recordDefinitionReferences>
                                                    <recordDefinitionReference name="UTVALG">
                                                        <fieldDefinitionReferences>
                                                            <fieldDefinitionReference name="UT.ID"/>
                                                        </fieldDefinitionReferences>
                                                    </recordDefinitionReference>
                                                </recordDefinitionReferences>
                                            </flatFileDefinitionReference>
                                            <relationType>n:1</relationType>
                                        </foreignKey>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="MD.UTVID"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                    <key name="for55_utvmote_moid_{$filnavn}">
                                        <foreignKey>
                                            <flatFileDefinitionReference name="UTVMOTE">
                                                <recordDefinitionReferences>
                                                    <recordDefinitionReference name="UTVMOTE">
                                                        <fieldDefinitionReferences>
                                                            <fieldDefinitionReference name="MO.ID"/>
                                                        </fieldDefinitionReferences>
                                                    </recordDefinitionReference>
                                                </recordDefinitionReferences>
                                            </flatFileDefinitionReference>
                                            <relationType>n:1</relationType>
                                        </foreignKey>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="MD.MOID"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                    <key name="for56_admindel_admid_{$filnavn}">
                                        <foreignKey>
                                            <flatFileDefinitionReference name="ADMINDEL">
                                                <recordDefinitionReferences>
                                                    <recordDefinitionReference name="ADMINDEL">
                                                        <fieldDefinitionReferences>
                                                            <fieldDefinitionReference name="AI.ID"/>
                                                        </fieldDefinitionReferences>
                                                    </recordDefinitionReference>
                                                </recordDefinitionReferences>
                                            </flatFileDefinitionReference>
                                            <relationType>n:1</relationType>
                                        </foreignKey>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="MD.ADMID"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                    <key name="for57_pernavn_sbhid_{$filnavn}">
                                        <foreignKey>
                                            <flatFileDefinitionReference name="PERNAVN">
                                                <recordDefinitionReferences>
                                                    <recordDefinitionReference name="PERNAVN">
                                                        <fieldDefinitionReferences>
                                                            <fieldDefinitionReference name="PN.ID"/>
                                                        </fieldDefinitionReferences>
                                                    </recordDefinitionReference>
                                                </recordDefinitionReferences>
                                            </flatFileDefinitionReference>
                                            <relationType>n:1</relationType>
                                        </foreignKey>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="MD.SBHID"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                </keys>
                            </xsl:when>
                            <xsl:when test="$filnavn='MOTEDOKLINK'">
                                <keys>
                                    <key name="prim_{$filnavn}">
                                        <primaryKey/>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="ML.MDID"/>
                                            <fieldDefinitionReference name="ML.RNR"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                    <key name="for58_pernavn_tkav_{$filnavn}">
                                        <foreignKey>
                                            <flatFileDefinitionReference name="PERNAVN">
                                                <recordDefinitionReferences>
                                                    <recordDefinitionReference name="PERNAVN">
                                                        <fieldDefinitionReferences>
                                                            <fieldDefinitionReference name="PN.ID"/>
                                                        </fieldDefinitionReferences>
                                                    </recordDefinitionReference>
                                                </recordDefinitionReferences>
                                            </flatFileDefinitionReference>
                                            <relationType>n:1</relationType>
                                        </foreignKey>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="ML.TKAV"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                </keys>
                            </xsl:when>
                            <xsl:when test="$filnavn='UTVTYPE'">
                                <keys>
                                    <key name="prim_{$filnavn}">
                                        <primaryKey/>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="UK.KODE"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                </keys>
                            </xsl:when>
                            <xsl:when test="$filnavn='UTVMEDLFUNK'">
                                <keys>
                                    <key name="prim_{$filnavn}">
                                        <primaryKey/>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="MK.KODE"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                </keys>
                            </xsl:when>
                            <xsl:when test="$filnavn='STATUSMDOK'">
                                <keys>
                                    <key name="prim_{$filnavn}">
                                        <primaryKey/>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="MS.STATUS"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                </keys>
                            </xsl:when>
                            <xsl:when test="$filnavn='UTVALGSAK'">
                                <keys>
                                    <key name="prim_{$filnavn}">
                                        <primaryKey/>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="US.ID"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                    <key name="for59_utvalg_utvid_{$filnavn}">
                                        <foreignKey>
                                            <flatFileDefinitionReference name="UTVALG">
                                                <recordDefinitionReferences>
                                                    <recordDefinitionReference name="UTVALG">
                                                        <fieldDefinitionReferences>
                                                            <fieldDefinitionReference name="UT.ID"/>
                                                        </fieldDefinitionReferences>
                                                    </recordDefinitionReference>
                                                </recordDefinitionReferences>
                                            </flatFileDefinitionReference>
                                            <relationType>n:1</relationType>
                                        </foreignKey>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="US.UTVID"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                </keys>
                            </xsl:when>
                            <xsl:when test="$filnavn='POLSAKSGANG'">
                                <keys>
                                    <key name="prim_{$filnavn}">
                                        <primaryKey/>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="SG.ID"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                </keys>
                            </xsl:when>
                            <xsl:when test="$filnavn='UTVBEHANDLING'">
                                <keys>
                                    <key name="prim_{$filnavn}">
                                        <primaryKey/>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="UB.ID"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                    <key name="for60_admindel_admid_{$filnavn}">
                                        <foreignKey>
                                            <flatFileDefinitionReference name="ADMINDEL">
                                                <recordDefinitionReferences>
                                                    <recordDefinitionReference name="ADMINDEL">
                                                        <fieldDefinitionReferences>
                                                            <fieldDefinitionReference name="AI.ID"/>
                                                        </fieldDefinitionReferences>
                                                    </recordDefinitionReference>
                                                </recordDefinitionReferences>
                                            </flatFileDefinitionReference>
                                            <relationType>n:1</relationType>
                                        </foreignKey>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="UB.ADMID"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                    <key name="for61_pernavn_sbhid_{$filnavn}">
                                        <foreignKey>
                                            <flatFileDefinitionReference name="PERNAVN">
                                                <recordDefinitionReferences>
                                                    <recordDefinitionReference name="PERNAVN">
                                                        <fieldDefinitionReferences>
                                                            <fieldDefinitionReference name="PN.ID"/>
                                                        </fieldDefinitionReferences>
                                                    </recordDefinitionReference>
                                                </recordDefinitionReferences>
                                            </flatFileDefinitionReference>
                                            <relationType>n:1</relationType>
                                        </foreignKey>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="UB.SBHID"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                </keys>
                            </xsl:when>
                            <xsl:when test="$filnavn='BEHAVHENG'">
                                <keys>
                                    <key name="prim_{$filnavn}">
                                        <primaryKey/>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="BA.UBID1"/>
                                            <fieldDefinitionReference name="BA.UBID2"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                </keys>
                            </xsl:when>
                            <xsl:when test="$filnavn='UTVBEHDOK'">
                                <keys>
                                    <key name="prim_{$filnavn}">
                                        <primaryKey/>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="BD.BEHID"/>
                                            <fieldDefinitionReference name="BD.DOKID"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                </keys>
                            </xsl:when>
                            <xsl:when test="$filnavn='UTVSAKSKART'">
                                <keys>
                                    <key name="prim_{$filnavn}">
                                        <primaryKey/>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="SK.UTVID"/>
                                            <fieldDefinitionReference name="SK.SAKSTYPE"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                </keys>
                            </xsl:when>
                            <xsl:when test="$filnavn='UTVSAKTYP'">
                                <keys>
                                    <key name="prim_{$filnavn}">
                                        <primaryKey/>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="SU.KODE"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                </keys>
                            </xsl:when>
                            <xsl:when test="$filnavn='UTDOKTYP'">
                                <keys>
                                    <key name="prim_{$filnavn}">
                                        <primaryKey/>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="DU.KODE"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                </keys>
                            </xsl:when>
                            <xsl:when test="$filnavn='UTVBEHSTAT'">
                                <keys>
                                    <key name="prim_{$filnavn}">
                                        <primaryKey/>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="BS.STATUS"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                </keys>
                            </xsl:when>
                            <xsl:when test="$filnavn='STDVERDI'">
                                <keys>
                                    <key name="prim_{$filnavn}">
                                        <primaryKey/>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="SV.ADMID"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                </keys>
                            </xsl:when>
                        </xsl:choose>
                        <fieldDefinitions>
                            <xsl:for-each select="TI.ATTR">
                                <xsl:variable name="feltnavn" select="."/>
                                <xsl:choose>
                                    <xsl:when test="$feltnavn='SA.ID'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='SA.SAAR'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='SA.SEKNR'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='SA.ADMID'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='SA.ANSVID'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='SA.TGGRUPPE'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='SA.ANTJP'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='SA.BEVTID'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='SA.UTLTIL'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='SA.DATO'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="date8"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='SA.SISTEJP'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="date8"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='SA.KASSDATO'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="date8"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='SA.OBS'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="date8"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='SA.UTLDATO'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="date8"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='SA.PAPIR'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="boolean"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='SA.U1'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="boolean"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='JF.ID'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='JF.FSAID'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='JF.FJPID'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='JF.TSAID'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='JF.TJPID'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='JF.TPSID'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='KL.SAID'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='KL.U1'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="boolean"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='SP.SAID'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='SP.U1'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="boolean"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='SS.MIDLERTIDIG'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="boolean"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='SS.LUKKET'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="boolean"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='SS.UTG'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="boolean"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='ST.UOFF'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="boolean"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='ST.Klageadg'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="boolean"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='JP.ID'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='JP.JAAR'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='JP.SEKNR'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='JP.SAID'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='JP.JPOSTNR'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='JP.JDATO'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="date8"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='JP.JDATO'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="date8"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='JP.UDATERT'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="boolean"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='JP.U1'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="boolean"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='JP.AVSKDATO'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="date8"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='JP.EKSPDATO'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="date8"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='JP.FORFDATO'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="date8"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='JP.OVDATO'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="date8"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='JP.AGDATO'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="date8"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='JP.TGGRUPPE'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='JP.U2'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="boolean"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='JP.PAPIR'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="boolean"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='JP.ANTVED'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='JP.UTLDATO'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="date8"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='JP.UTLTIL'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='AM.ID'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='AM.JPID'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='AM.IHTYPE'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="boolean"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='AM.KOPIMOT'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="boolean"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='AM.BEHANSV'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="boolean"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='AM.GRUPPEMOT'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="boolean"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='AM.U1'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="boolean"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='AM.ADMID'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='AM.SBHID'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='AM.AVSKAV'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='AM.AVSKDATO'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="date8"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='AM.BESVAR'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='AM.FRIST'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="date8"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='ER.JPID'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='ER.ASAAR'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='ER.ASNR'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='ER.ABASEID'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='ER.AJPID'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='ER.ADMID'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='ER.SBHID'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='ND.EKSTPROD'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="boolean"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='ND.OPPF'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="boolean"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='JS.EKSPEDERT'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="boolean"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='JS.FORARKIV'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="boolean"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='JS.FORLEDER'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="boolean"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='JS.FORSAKSBEH'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="boolean"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='JS.FOREKST'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="boolean"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='JS.FORINT'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="boolean"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='AV.MIDLERTID'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="boolean"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='AV.BESVART'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="boolean"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='PS.ID'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='PS.SAID'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='PS.JPID'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='PS.DATO'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="date8"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='PS.FORELDET'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="boolean"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='PS.ADMID'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='PS.TGGRUPPE'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='PS.DOKID'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='PH.FPSID'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='PH.TPSID'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='PH.TOVEIS'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="boolean"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='EP.PSID'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='EP.SORDFLAGG'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="boolean"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='PL.PSID'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='AK.ADRID'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='AK.OVERORD'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='AK.ADRGRUPPE'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="boolean"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='AK.TILDATO'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="date8"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='AK.TGGRUPPE'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='AK.OPDGRUPPE'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='AK.BASEID'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='AK.REGEPOST'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="boolean"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='MG.GRID'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='MG.MEDLID'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='PO.OPRDATO'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="date8"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='PO.NEDLDATO'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="date8"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='KA.ADRID'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='KA.FRADATO'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="date8"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='KA.TILDATO'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="date8"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='KA.AUTAV'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='KA.AUTDATO'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="date8"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='SE.ID'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='SE.ADRID'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='SE.FRADATO'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="date8"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='SE.TILDATO'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="date8"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='SE.VERAV'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='SE.VERDATO'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="date8"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='KF.ADRID'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='ME.ID'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='ME.SAID'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='ME.JPID'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='ME.DOKID'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='ME.DOKVER'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='ME.RNR'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='ME.TGGRUPPE'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='ME.OPPBEDATO'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="date8"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='ME.REGDATO'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="date8"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='ME.REGKL'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="time"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='ME.REGAV'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='ME.PVGAV'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='ME.REFDOKID'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='TI.ID'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='TI.SAID'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='TI.JPID'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='TI.DOKID'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='TI.DOKVER'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='TI.RNR'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='TI.TGGRUPPE'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='TI.OPPBEDATO'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="date8"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='TI.REGDATO'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="date8"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='TI.REGKL'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="time"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='TI.REGAV'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='TI.PVGAV'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='TI.REFDOKID'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='IT.MERKNAD'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="boolean"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='IT.AUTOLOG'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="boolean"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='DL.JPID'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='DL.RNR'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='DL.DOKID'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='DL.TKDATO'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="date8"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='DL.TKAV'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='DB.DOKID'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='DB.PAPIR'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="boolean"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='DB.UTARBAV'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='DB.AGDATO'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="date8"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='DB.TGGRUPPE'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='VE.DOKID'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='VE.VERSJON'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='VE.AKTIV'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="boolean"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='VE.REGAV'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='VE.OPPBEDATO'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="date8"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='DI.DOKID'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='DI.VERSJON'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='DI.SIGNNR'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='DI.DOKSIGN'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="boolean"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='DI.ARKSIGN'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="boolean"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='DI.FORSIGN'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="boolean"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='DI.SIGVERAV'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='DI.SIGVERDATO'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="date8"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='ED.DOKID'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='ED.SORDFLAGG'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="boolean"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='DT.JOURNAL'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="boolean"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='DT.MOTEDOK'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="boolean"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='LF.ARKIV'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="boolean"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='AI.ID'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='AI.IDFAR'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='AI.FRADATO'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="date8"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='AI.TILDATO'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="date8"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='AL.ADMIDFRA'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='AL.ADMIDTIL'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='AA.ADMID'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='AA.ADRID'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='AP.PERIODE'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='AP.FRADATO'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="date8"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='AP.TILDATO'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="date8"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='AR.NUMSER'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='AR.ABASEID'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='AR.FRADATO'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="date8"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='AR.TILDATO'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="date8"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='AD.PERIODE'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='AD.SPEFSAK'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="boolean"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='AD.SPEFDOK'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="boolean"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='AD.LUKKET'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="boolean"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='AD.PAPIR'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="boolean"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='AD.ELDOK'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="boolean"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='AD.NUMSER'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='AD.FRISEK'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="boolean"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='AD.FRADATO'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="date8"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='AD.TILDATO'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="date8"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='AD.EKSPDATO'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="date8"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='AR.KONTRDATO'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="date8"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='AD.KONTRAV'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='OP.OVBESK'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="boolean"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='OP.KLFLAGG'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="boolean"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='OP.SIFLAGG'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="boolean"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='OP.EVOK'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="boolean"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='OP.EVAUTO'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="boolean"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='OP.SEKFLAGG'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="boolean"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='OP.FRADATO'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="date8"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='OP.TILDATO'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="date8"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='OP.MAKSLEN'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='OV.REGFLAGG'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="boolean"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='OV.SEKFLAGG'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="boolean"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='OV.BEVTID'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='EA.SORDFLAGG'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="boolean"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='JE.TILDATO'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="date8"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='NU.ID'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='NU.AAR'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='NU.SEKNR1'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='NU.SEKNR2'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='NU.AARAUTO'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="boolean"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='AS.SPEFSAK'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="boolean"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='AS.SPEFDOK'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="boolean"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='AS.LUKKET'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="boolean"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='PE.ID'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='PE.FRADATO'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="date8"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='PE.TILDATO'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="date8"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='PN.ID'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='PN.PEID'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='PN.AKTIV'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="boolean"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='PN.FRADATO'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="date8"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='PN.TILDATO'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="date8"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='PA.PEID'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='PA.ADRID'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='PR.ID'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='PR.PEID'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='PR.ROLLEID'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='PR.STDROLLE'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="boolean"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='PR.ADMID'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='PR.FRADATO'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="date8"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='PR.TILDATO'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="date8"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='FR.PRID'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='FR.FUNGPRID'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='RO.ROLLEID'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='RO.SOK'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="boolean"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='RO.SYSADM'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="boolean"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='RO.ARKLEDER'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="boolean"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='RO.LEDER'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="boolean"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='RO.SAKSBH'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="boolean"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='RO.UTVSEKR'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="boolean"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='RO.REGUTVMED'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="boolean"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='RO.DRIFT'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="boolean"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='RO.REGROLLER'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="boolean"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='RO.REGPERSON'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="boolean"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='RO.REGARK'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="boolean"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='RO.REGADR'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="boolean"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='TJ.PEID'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='TJ.ADMID'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='TJ.AUTAV'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='TJ.FRADATO'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="date8"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='TJ.TILDATO'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="date8"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='TJ.AUTOPPAV'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='TK.SERIE'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='TK.RANG'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='TK.FRADATO'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="date8"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='TK.TILDATO'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="date8"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='TK.EPOSTNIV'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='TH.AGAAR'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='TH.AGDAGER'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='KT.PEID'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='KT.KLAV'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='KT.KLOPPHAV'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='PT.PEID'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='PT.ADMID'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='PT.AUTAV'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='PT.FRADATO'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="date8"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='PT.TILDATO'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="date8"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='PT.AUTOPPAV'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='TG.GRUPPEID'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='TG.GENERELL'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="boolean"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='TG.OPPRAV'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='TG.FRADATO'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="date8"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='TG.TILDATO'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="date8"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='PG.PEID'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='PG.GRUPPEID'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='PG.PERROLID'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='PG.MEDKOD'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='PG.INNMAV'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='PG.FRADATO'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="date8"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='PG.TILDATO'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="date8"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='PG.UTMAV'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='UT.ID'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='UT.ADMID'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='UT.FRADATO'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="date8"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='UT.TILDATO'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="date8"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='UT.OPPNAV'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='UT.OPPNAVUTV'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='UT.OPPNDATO'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="date8"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='UT.ANTMEDL'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='UT.ETABLERT'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="date8"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='UT.NEDLAGT'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="date8"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='UT.ADRID'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='UT.MONUMSER'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='UT.SAMMENR'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="boolean"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='MO.ID'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='MO.FORTS'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='MO.NR'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='MO.UTVID'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='MO.LUKKET'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="boolean"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='MO.DATO'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="date8"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='MO.START'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="time"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='MO.SLUTT'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="time"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='MO.FRIST'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="date8"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='MO.SAKSKART'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="boolean"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='MO.PROTOKOLL'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="boolean"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='UM.UTVID'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='UM.PNID'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='UM.RANGERING'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='UM.VARAFOR'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='UM.FRADATO'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="date8"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='UM.TILDATO'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="date8"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='UM.SORT'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='UM.REPRES'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='MO.SEKR'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="boolean"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='MO.MEDLEM'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="boolean"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='FU.MOID'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='FU.PNID'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='FU.SORT'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='FU.VARAFOR'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='FU.SEKR'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="boolean"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='MD.ID'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='MD.UTVID'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='MD.MOID'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='MD.DATO'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="date8"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='MD.PAPIRDOK'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="boolean"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='MD.U1'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="boolean"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='MD.ADMID'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='MD.SBHID'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='MD.TGGRUPPE'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='MD.AGDATO'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="date8"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='MD.BEVTID'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='MD.KASSDATO'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="date8"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='MI.MDPID'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='MI.RNR'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='MI.DOKID'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='MI.TKDATO'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="date8"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='MI.TKAV'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='MK.TALE'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="boolean"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='MK.MEDLEM'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="boolean"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='MK.SEKR'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="boolean"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='MK.FMKODE'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="boolean"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='US.UTVID'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='US.ID'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='US.U1'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="boolean"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='US.LUKKET'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="boolean"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='US.TGGRUPPE'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='US.SAMMENR'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="boolean"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='US.SAID'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='US.POLSGID'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='US.JPID'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='SG.ID'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='SG.SAID'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='SG.KLADGANG'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="boolean"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='SG.LUKKET'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="boolean"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='SG.STARTDATO'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="date8"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='SG.VEDTDATO'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="date8"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='SG.SISTEVEDT'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='UB.ID'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='UB.UTSAKID'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='UB.RFOLGE'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='UB.MOID'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='UB.USEKNR'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='UB.AAR'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='UB.ADMID'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='UB.SBHID'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='UB.PROTOKOLL'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="boolean"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='BA.UBID1'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='BA.UBID2'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='BD.BEHID'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='BD.DOKID'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='BD.JPID'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='SK.UTVID'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='SK.SORT'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='SK.NUMSER'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='SK.OVSKR'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="boolean"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='SK.KUNOVSKR'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="boolean"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='SK.UTVSNR'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="boolean"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='SK.UTVSTIT'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="boolean"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='SK.ARKSNR'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="boolean"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='SK.ARKSDNR'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="boolean"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='SK.DOKIH'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="boolean"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='SK.DOKDATO'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="boolean"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='SK.AVS'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="boolean"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='DU.MOTEDOK'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="boolean"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='DU.AUTOJOUR'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="boolean"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='DU.ALDRIJOUR'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="boolean"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='BS.KOLISTE'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="boolean"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='BS.KANSKART'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="boolean"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='BS.SAKSKART'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="boolean"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='BS.BEHANDLET'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="boolean"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='BS.SORT1'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='SV.ADMID'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='SV.BFRIST1'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='SV.BFRIST2'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='SV.BFFRIST1'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='SV.BFFRIST2'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='SV.ARKFAVSL'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="boolean"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='SV.EKSTDOKTG'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="boolean"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='SV.BEVER'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='EI.ORGNR'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='EI.KOMMUNE'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='EI.BASEID'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='EI.FRADATO'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="date8"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='EI.TILDATO'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="date8"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='EI.PRODDATO'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="date8"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='TI.ANTFILER'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='TI.FILDEL'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:when test="$feltnavn='TI.ANTPOSTER'">
                                        <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                                    </xsl:when>
                                    <xsl:otherwise>
                                        <fieldDefinition name="{$feltnavn}" typeReference="string"/>
                                    </xsl:otherwise>
                                </xsl:choose>
                            </xsl:for-each>
                        </fieldDefinitions>
                    </recordDefinition>
                </recordDefinitions>
            </flatFileDefinition>
<!--        </flatFileDefinitions>
        <structureTypes>
            <flatFileTypes>
                <flatFileType name="filref"/>
            </flatFileTypes>
            <recordTypes>
                <recordType name="recref"/>
            </recordTypes>
            <fieldTypes>
                <fieldType name="string">
                    <dataType>String</dataType>
                </fieldType>
            </fieldTypes>
        </structureTypes>-->
    </xsl:template>

    <xsl:template match="TI.ATTR">0
        <fieldDefinitions>
            <xsl:variable name="feltnavn">
                <value><xsl:value-of select="TI.ATTR"/></value>
            </xsl:variable>
            <xsl:choose>
                <xsl:when test="$feltnavn='SA.ID'">
                    <fieldDefinition name="{$feltnavn}" typeReference="integer"/>
                </xsl:when>
                <xsl:when test="$feltnavn='SA.DATO'">
                    <fieldDefinition name="{$feltnavn}" typeReference="date8"/>
                </xsl:when>
                <xsl:otherwise>
                    <fieldDefinition name="{$feltnavn}" typeReference="string"/>
                </xsl:otherwise>
            </xsl:choose>
        </fieldDefinitions>
    </xsl:template>
</xsl:stylesheet>