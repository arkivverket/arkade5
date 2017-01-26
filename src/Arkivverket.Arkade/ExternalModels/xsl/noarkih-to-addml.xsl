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
