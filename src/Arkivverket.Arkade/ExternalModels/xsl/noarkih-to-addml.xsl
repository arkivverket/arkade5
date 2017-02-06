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
                                <fieldFormat>ddmmyyyy</fieldFormat>
                            </fieldType>
                            <fieldType name="time">
                                <dataType>time</dataType>
                                <fieldFormat>hhmmss</fieldFormat>
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
                            <additionalElement name="date">
                                <value><xsl:value-of select="EI.TILDATO"/></value>
                            </additionalElement>
                            <additionalElement name="type">
                                <value>Noark 4</value>
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
                                <value>Noark 4</value>
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
                    <value><xsl:value-of select="FIL/TI.FILNAVN"/></value>
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
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="SA.ARKDEL"/>
                                            <fieldDefinitionReference name="SA.SAAR"/>
                                            <fieldDefinitionReference name="SA.SEKNR"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                    <key name="for_admindel_{$filnavn}">
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
                                        </foreignKey>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="SA.ADMID"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                    <key name="for_pernavn_ansv_{$filnavn}">
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
                                        </foreignKey>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="SA.ANSVID"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                    <key name="for_pernavn_utlev_{$filnavn}">
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
                            <xsl:when test="$filnavn='KLASS'">
                                <keys>
                                    <key name="prim_{$filnavn}">
                                        <primaryKey/>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="KL.SAID"/>
                                            <fieldDefinitionReference name="KL.ORDNPRI"/>
                                            <fieldDefinitionReference name="KL.ORDNVER"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                </keys>
                            </xsl:when>
                            <xsl:when test="$filnavn='SAKPART'">
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
                            <xsl:when test="$filnavn='SAKSTAT'">
                                <keys>
                                    <key name="prim_{$filnavn}">
                                        <primaryKey/>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="SS.STATUS"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                </keys>
                            </xsl:when>
                            <xsl:when test="$filnavn='SAKTYPE'">
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
                            <xsl:when test="$filnavn='JOURNPST'">
                                <keys>
                                    <key name="prim_{$filnavn}">
                                        <primaryKey/>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="JP.ID"/>
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
                                </keys>
                            </xsl:when>
                            <xsl:when test="$filnavn='ESAKREF'">
                                <keys>
                                    <key name="prim_{$filnavn}">
                                        <primaryKey/>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="ER.JPID"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                </keys>
                            </xsl:when>
                            <xsl:when test="$filnavn='DOKTYPE'">
                                <keys>
                                    <key name="prim_{$filnavn}">
                                        <primaryKey/>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="ND.DOKTYPE"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                </keys>
                            </xsl:when>
                            <xsl:when test="$filnavn='JOURNSTA'">
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
                            <xsl:when test="$filnavn='AVGRKODE'">
                                <keys>
                                    <key name="prim_{$filnavn}">
                                        <primaryKey/>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="AG.KODE"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                </keys>
                            </xsl:when>
                            <xsl:when test="$filnavn='AVSKRM'">
                                <keys>
                                    <key name="prim_{$filnavn}">
                                        <primaryKey/>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="AV.KODE"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                </keys>
                            </xsl:when>
                            <xsl:when test="$filnavn='FORSMATE'">
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
                            <xsl:when test="$filnavn='PRES'">
                                <keys>
                                    <key name="prim_{$filnavn}">
                                        <primaryKey/>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="PS.ID"/>
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
                            <xsl:when test="$filnavn='EOPRES'">
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
                            <xsl:when test="$filnavn='LFPRES'">
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
                            <xsl:when test="$filnavn='ADRESSEK'">
                                <keys>
                                    <key name="prim_{$filnavn}">
                                        <primaryKey/>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="AK.ADRID"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                </keys>
                            </xsl:when>
                            <xsl:when test="$filnavn='ADRESSEK'">
                                <keys>
                                    <key name="alt_{$filnavn}">
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="AK.KORTNAVN"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                </keys>
                            </xsl:when>
                            <xsl:when test="$filnavn='MEDADRGR'">
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
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="PO.POSTNR"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                </keys>
                            </xsl:when>
                            <xsl:when test="$filnavn='KPAUT'">
                                <keys>
                                    <key name="prim_{$filnavn}">
                                        <primaryKey/>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="KA.ADRID"/>
                                            <fieldDefinitionReference name="KA.TGKODE"/>
                                            <fieldDefinitionReference name="KA.TILDATO"/>
                                        </fieldDefinitionReferences>
                                    </key>
                                </keys>
                            </xsl:when>
                            <xsl:when test="$filnavn='DOGISERT'">
                                <keys>
                                    <key name="prim_{$filnavn}">
                                        <primaryKey/>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="SE.ID"/>
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
                                </keys>
                            </xsl:when>
                            <xsl:when test="$filnavn='TILLEGG'">
                                <keys>
                                    <key name="prim_{$filnavn}">
                                        <primaryKey/>
                                        <fieldDefinitionReferences>
                                            <fieldDefinitionReference name="TI.ID"/>
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
