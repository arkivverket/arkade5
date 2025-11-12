<?xml version="1.0" encoding="iso-8859-1"?>
<!--
=== SiardMetaToXhtml.xsl ===============================================
Main script.
Version     : $Id: metadata.xsl 1208 2010-06-22 07:41:00Z hartwig $
Application : Swiss Federal Archive SIARD v2.x
Description : XS transformation to transform metadata xml to xhtml.
Platform    : Xsl transformer. Implemented and tested with Xalan.
========================================================================
Copyright  : Swiss Federal Archives, Berne, Switzerland, 2008
Created    : 16.06.2008, Niklaus Aeschbacher, Enter AG, Zurich
========================================================================
-->
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
  xmlns:siard="http://www.bar.admin.ch/xmlns/siard/1.0/metadata.xsd"
  xmlns:html="http://www.w3.org/1999/xhtml"
  xmlns="http://www.w3.org/1999/xhtml" exclude-result-prefixes="html"
  version="2.0">
  
  <xsl:variable name="quote">
    "
  </xsl:variable>
  <xsl:variable name="at">
    @
  </xsl:variable>
  <xsl:output method="xml" indent="yes" encoding="iso-8859-1"
    doctype-system="http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"
    doctype-public="-//W3C//DTD XHTML 1.0 Transitional//EN"/>
  
  <xsl:template match="text()" name="remove-quotes">
    <xsl:param name="input" select="."/>
    
    <xsl:choose>
      <xsl:when test="contains($input,$quote)">
        <xsl:call-template name="remove-quotes">
          <xsl:with-param name="input"
            select="concat(substring-before($input,$quote),'',substring-after($input,$quote))"/>
        </xsl:call-template>
      </xsl:when>
      <xsl:otherwise>
        <xsl:value-of select="$input"/>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>
  
  <xsl:template match="text()" name="remove-ats">
    <xsl:param name="input" select="."/>
    
    <xsl:choose>
      <xsl:when test="contains($input,$at)">
        <xsl:call-template name="remove-ats">
          <xsl:with-param name="input"
            select="concat(substring-before($input,$at),'',substring-after($input,$at))"/>
        </xsl:call-template>
      </xsl:when>
      <xsl:otherwise>
        <xsl:value-of select="$input"/>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>
  
  <xsl:template match="/siard:siardArchive">
    <html xmlns="http://www.w3.org/1999/xhtml">
      <head>
        <title>
          SIARD File Content
        </title>
        <style type="text/css">
          <!-- Common table  styles -->
          
          .tableTitle { background-color: #999999; color: #FFFFFF;
          text-align: center;
          
          }
          
          .tableTitleDark { text-align: center; font-weight: bold;
          
          }
          
          .horizontalTitleColumn { text-align: left; }
          
          table.light { border-width: 1px;
          
          border-style: none; border-color: gray; border-collapse: separate;
          background-color: white; } table.light th { border-width: 1px;
          padding: 1px; border-style: ridge; border-color: white;
          background-color: #AFEEEE;
          
          } table.light td { border-width: 1px; padding: 1px; vertical-align:
          text-top; border-style: ridge; border-color: white;
          background-color: #F0F8FF;
          
          }
          
          table.strong { border-width: 1px;
          
          border-style: none; border-color: gray; border-collapse: separate;
          background-color: white; } table.strong th { border-width: 1px;
          padding: 1px; border-style: ridge; border-color: white;
          background-color: #9999FF;
          
          }
          
          table.strong td { vertical-align: text-top; border-width: 1px;
          padding: 1px; border-style: ridge; border-color: white;
          background-color: #99CCFF;
          
          }
          
          table.medium { border-width: 1px;
          
          border-style: none; border-color: gray; border-collapse: separate;
          background-color: white; }
          
          table.medium th { border-width: 1px; padding: 1px; border-style:
          ridge; border-color: white; background-color: #FFE4E1;
          
          }
          
          table.medium td { border-width: 1px; padding: 1px; vertical-align:
          text-top; border-style: ridge; border-color: white;
          background-color: #FFF0F5;
          
          }
          
          table.title { border-width: 1px;
          
          border-style: none; border-color: gray; border-collapse: separate;
          background-color: white; }
          
          table.title th { border-width: 1px; padding: 1px; border-style:
          ridge; border-color: white; background-color: #F5F5F5;
          
          }
          
          table.title td { border-width: 1px; padding: 1px; vertical-align:
          text-top; border-style: ridge; border-color: white;
          background-color: #F8F8FF;
          
          }
          
          <!-- Data base description styles -->
          .databaseTitleColumn { text-align: right; } .databaseValueColumn {
          text-align: left; }
          
          <!-- Schema description styles -->
          .schemaTitleColumn { text-align: right; } .schemaValueColumn {
          text-align: left; }
          
          body.common { font-family: Verdana,Arial, Helvetica, sans-serif;
          font-size: 8pt; color: #555555; }
          
          h1.small { font-size: 14pt; }
          
          h2.small { font-size: 10pt; }
          
          h3.small { font-size: 8pt; }
          
        </style>
      </head>
      <body class="common">
        <h1 class="small">
          SIARD File Content
        </h1>
        <h2 class="small">
          <xsl:value-of select="siard:dbname"/>
        </h2>
        <table class="title">
          <tr>
            <td colspan="2" class="tableTitle">
              DATA BASE
            </td>
          </tr>
          <tr>
            <th class="databaseTitleColumn">
              <xsl:text>
                Name
              </xsl:text>
            </th>
            <th class="databaseValueColumn">
              <xsl:value-of select="siard:dbname"/>
            </th>
          </tr>
          <tr>
            <td class="databaseTitleColumn">
              <xsl:text>
                Version
              </xsl:text>
            </td>
            <td class="databaseValueColumn">
              <xsl:value-of select="@version"/>
            </td>
          </tr>
          <tr>
            <td class="databaseTitleColumn">
              <xsl:text>
                Description
              </xsl:text>
            </td>
            <td class="databaseValueColumn">
              <xsl:value-of select="siard:description"/>
            </td>
          </tr>
          <tr>
            <td class="databaseTitleColumn">
              <xsl:text>
                Archiver
              </xsl:text>
            </td>
            <td class="databaseValueColumn">
              <xsl:value-of select="siard:archiver"/>
            </td>
          </tr>
          <tr>
            <td class="databaseTitleColumn">
              <xsl:text>
                Archiver Contact
              </xsl:text>
            </td>
            <td class="databaseValueColumn">
              <xsl:value-of select="siard:archiverContact"/>
            </td>
          </tr>
          <tr>
            <td class="databaseTitleColumn">
              <xsl:text>
                Data owner
              </xsl:text>
            </td>
            <td class="databaseValueColumn">
              <xsl:value-of select="siard:dataOwner"/>
            </td>
          </tr>
          <tr>
            <td class="databaseTitleColumn">
              <xsl:text>
                Data origin timespan
              </xsl:text>
            </td>
            <td class="databaseValueColumn">
              <xsl:value-of select="siard:dataOriginTimespan"/>
            </td>
          </tr>
          <tr>
            <td class="databaseTitleColumn">
              <xsl:text>
                Archival date
              </xsl:text>
            </td>
            <td class="databaseValueColumn">
              <xsl:value-of select="siard:archivalDate"/>
            </td>
          </tr>
          <tr>
            <td class="databaseTitleColumn">
              <xsl:text>
                Message digest
              </xsl:text>
            </td>
            <td class="databaseValueColumn">
              <xsl:value-of select="siard:messageDigest"/>
            </td>
          </tr>
          <tr>
            <td class="databaseTitleColumn">
              <xsl:text>
                Client machine
              </xsl:text>
            </td>
            <td class="databaseValueColumn">
              <xsl:value-of select="siard:clientMachine"/>
            </td>
          </tr>
          <tr>
            <td class="databaseTitleColumn">
              <xsl:text>
                Producing application
              </xsl:text>
            </td>
            <td class="databaseValueColumn">
              <xsl:value-of select="siard:producerApplication"/>
            </td>
          </tr>
          <tr>
            <td class="databaseTitleColumn">
              <xsl:text>
                Database product
              </xsl:text>
            </td>
            <td class="databaseValueColumn">
              <xsl:value-of select="siard:databaseProduct"/>
            </td>
          </tr>
          <tr>
            <td class="databaseTitleColumn">
              <xsl:text>
                Connection
              </xsl:text>
            </td>
            <td class="databaseValueColumn">
              <xsl:value-of select="siard:connection"/>
            </td>
          </tr>
          <tr>
            <td class="databaseTitleColumn">
              <xsl:text>
                Data base user
              </xsl:text>
            </td>
            <td class="databaseValueColumn">
              <xsl:value-of select="siard:databaseUser"/>
            </td>
          </tr>
        </table>
        
        <!-- Table of contents -->
        <h2>
          <xsl:text>
            Table of contents
          </xsl:text>
        </h2>
        <xsl:if test="count(siard:schemas/siard:schema) &gt; 0">
          <ul>
            <li>
              <h2 class="small">
                Schemas
              </h2>
              <xsl:for-each select="siard:schemas/siard:schema">
                <p/>
                
                <!--  <xsl:variable name="schemaAName" select='replace(siard:name, "\W","")'/> -->
                <xsl:variable name="schemaAName">
                  <xsl:call-template name="remove-quotes">
                    <xsl:with-param name="input" select="siard:name"/>
                  </xsl:call-template>
                </xsl:variable>
                <xsl:variable name="schemaName">
                  <xsl:call-template name="remove-quotes">
                    <xsl:with-param name="input" select="siard:name"/>
                  </xsl:call-template>
                </xsl:variable>
                <ul>
                  <li>
                    <a href="#{$schemaAName}">
                      <xsl:value-of select="$schemaName"/>
                    </a>
                    <p/>
                    <xsl:if test="count(siard:tables/siard:table) &gt; 0">
                      <ul>
                        <li>
                          <h3 class="small">
                            Tables
                          </h3>
                          <xsl:for-each select="siard:tables/siard:table">
                            <p/>
                            
                            <xsl:variable name="tableAName">
                              <xsl:call-template name="remove-quotes">
                                <xsl:with-param name="input" select="siard:name"/>
                              </xsl:call-template>
                            </xsl:variable>
                            <xsl:variable name="tableName">
                              <xsl:call-template name="remove-quotes">
                                <xsl:with-param name="input" select="siard:name"/>
                              </xsl:call-template>
                            </xsl:variable>
                            
                            <a href="#{$schemaAName}.{$tableAName}">
                              <xsl:value-of select="$tableName"/>
                            </a>
                            <p/>
                          </xsl:for-each>
                        </li>
                      </ul>
                    </xsl:if>
                    
                    <xsl:if test="count(siard:views/siard:view) &gt; 0">
                      <ul>
                        <li>
                          <h3 class="small">
                            Views
                          </h3>
                          <xsl:for-each select="siard:views/siard:view">
                            <p/>
                            <xsl:variable name="viewAName">
                              <xsl:call-template name="remove-quotes">
                                <xsl:with-param name="input" select="siard:name"/>
                              </xsl:call-template>
                            </xsl:variable>
                            <xsl:variable name="viewName" select="siard:name"/>
                            <a href="#{$schemaAName}.{$viewAName}">
                              <xsl:value-of select="$viewName"/>
                            </a>
                            <p/>
                          </xsl:for-each>
                        </li>
                      </ul>
                    </xsl:if>
                    
                    <xsl:if test="count(siard:routines/siard:routine) &gt; 0">
                      <ul>
                        <li>
                          <h3 class="small">
                            Routines
                          </h3>
                          <xsl:for-each select="siard:routines/siard:routine">
                            <p/>
                            <xsl:variable name="routineAName">
                              <xsl:call-template name="remove-quotes">
                                <xsl:with-param name="input" select="siard:name"/>
                              </xsl:call-template>
                            </xsl:variable>
                            <xsl:variable name="routineName" select="siard:name"/>
                            <a href="#{$schemaAName}.{$routineAName}">
                              <xsl:value-of select="$routineName"/>
                            </a>
                          </xsl:for-each>
                        </li>
                      </ul>
                    </xsl:if>
                  </li>
                </ul>
              </xsl:for-each>
            </li>
          </ul>
        </xsl:if>
        <ul>
          <li>
            <h2 class="small">
              <a href="#users">
                Users
              </a>
            </h2>
          </li>
          <li>
            <h2 class="small">
              <a href="#roles">
                Roles
              </a>
            </h2>
          </li>
          <li>
            <h2 class="small">
              <a href="#privileges">
                Privileges
              </a>
            </h2>
          </li>
        </ul>
        
        <!-- The content -->
        <h2>
          Contents
        </h2>
        <xsl:if test="count(siard:schemas/siard:schema) &gt; 0">
          <ul>
            <li>
              <h2 class="small">
                Schemas
              </h2>
              <xsl:for-each select="siard:schemas/siard:schema">
                <p/>
                <!-- <xsl:variable name="schemaName" select="siard:name"/> -->
                <xsl:variable name="schemaAName">
                  <xsl:call-template name="remove-quotes">
                    <xsl:with-param name="input" select="siard:name"/>
                  </xsl:call-template>
                </xsl:variable>
                <xsl:variable name="schemaName" select="siard:name"/>
                <ul>
                  <li>
                    <table class="strong" width="100%">
                      <tr>
                        <td colspan="2" class="tableTitleDark">
                          SCHEMA
                          <a name="{$schemaAName}"/>
                        </td>
                      </tr>
                    </table>
                    <table class="strong">
                      <tr>
                        <th class="horizontalTitleColumn">
                          Name
                        </th>
                        <th class="horizontalTitleColumn">
                          Folder
                        </th>
                      </tr>
                      <tr>
                        <td class="schemaValueColumn">
                          <xsl:value-of select="$schemaName"/>
                        </td>
                        <td class="schemaValueColumn">
                          <xsl:value-of select="siard:folder"/>
                        </td>
                      </tr>
                    </table>
                    <p/>
                    <xsl:if test="count(siard:tables/siard:table) &gt; 0">
                      <ul>
                        <li>
                          <h3 class="small">
                            Tables
                          </h3>
                          <xsl:for-each select="siard:tables/siard:table">
                            <p/>
                            <xsl:variable name="tableAName">
                              <xsl:call-template name="remove-quotes">
                                <xsl:with-param name="input" select="siard:name"/>
                              </xsl:call-template>
                            </xsl:variable>
                            <xsl:variable name="tableName" select="siard:name"/>
                            <table class="medium">
                              <tr>
                                <td colspan="4" class="tableTitleDark">
                                  TABLE
                                  <a name="{$schemaAName}.{$tableAName}"/>
                                </td>
                              </tr>
                              <tr>
                                <th class="horizontalTitleColumn">
                                  Name
                                </th>
                                <th class="horizontalTitleColumn">
                                  Folder
                                </th>
                                <th class="horizontalTitleColumn">
                                  Rows
                                </th>
                                <th class="horizontalTitleColumn">
                                  Description
                                </th>
                              </tr>
                              <tr>
                                <td class="schemaValueColumn">
                                  <xsl:value-of select="$tableName"/>
                                </td>
                                <td class="schemaValueColumn">
                                  <xsl:value-of select="siard:folder"/>
                                </td>
                                <td class="schemaValueColumn">
                                  <xsl:value-of select="siard:rows"/>
                                </td>
                                <td class="schemaValueColumn">
                                  <xsl:value-of select="siard:description"/>
                                </td>
                              </tr>
                            </table>
                            <p/>
                            <xsl:if test="count(siard:columns/siard:column) &gt; 0">
                              <ul>
                                <li>
                                  <h3 class="small">
                                    Columns
                                  </h3>
                                  <table class="light">
                                    <tr>
                                      <th class="horizontalTitleColumn">
                                        Name
                                      </th>
                                      <th class="horizontalTitleColumn">
                                        Folder
                                      </th>
                                      <th class="horizontalTitleColumn">
                                        Type
                                      </th>
                                      <th class="horizontalTitleColumn">
                                        Original type
                                      </th>
                                      <th class="horizontalTitleColumn">
                                        Default value
                                      </th>
                                      <th class="horizontalTitleColumn">
                                        Nullable
                                      </th>
                                      <th class="horizontalTitleColumn">
                                        Description
                                      </th>
                                    </tr>
                                    <xsl:for-each select="siard:columns/siard:column">
                                      <tr>
                                        <td class="schemaValueColumn">
                                          <xsl:variable name="columnAName">
                                            <xsl:call-template name="remove-quotes">
                                              <xsl:with-param name="input" select="siard:name"/>
                                            </xsl:call-template>
                                          </xsl:variable>
                                          <xsl:variable name="columnName"
                                            select="siard:name"/>
                                          <xsl:value-of select="$columnName"/>
                                          
                                          <a
                                            name="{$schemaAName}.{$tableAName}.{$columnAName}"/>
                                        </td>
                                        <td class="schemaValueColumn">
                                          <xsl:value-of select="siard:folder"/>
                                        </td>
                                        <td class="schemaValueColumn">
                                          <xsl:value-of select="siard:type"/>
                                        </td>
                                        <td class="schemaValueColumn">
                                          <xsl:value-of select="siard:typeOriginal"/>
                                        </td>
                                        <td class="schemaValueColumn">
                                          <xsl:value-of select="siard:defaultValue"/>
                                        </td>
                                        <td class="schemaValueColumn">
                                          <xsl:value-of select="siard:nullable"/>
                                        </td>
                                        <td class="schemaValueColumn">
                                          <xsl:value-of select="siard:description"/>
                                        </td>
                                      </tr>
                                    </xsl:for-each>
                                  </table>
                                </li>
                              </ul>
                            </xsl:if>
                            <p/>
                            <xsl:if test="count(siard:primaryKey) &gt; 0">
                              <ul>
                                <li>
                                  <h3 class="small">
                                    Primary key
                                  </h3>
                                  <table class="light">
                                    <tr>
                                      <th class="horizontalTitleColumn">
                                        Name
                                      </th>
                                      <th class="horizontalTitleColumn">
                                        Column(s)
                                      </th>
                                      <th class="horizontalTitleColumn">
                                        Description
                                      </th>
                                    </tr>
                                    <tr>
                                      <td class="schemaValueColumn">
                                        <xsl:variable name="primaryKey"
                                          select="siard:primaryKey/siard:name"/>
                                        <xsl:variable name="primaryKeyAName">
                                          <xsl:call-template name="remove-quotes">
                                            <xsl:with-param name="input"
                                              select="siard:primaryKey/siard:name"/>
                                          </xsl:call-template>
                                        </xsl:variable>
                                        <a
                                          name="{$schemaAName}.{$tableAName}.{$primaryKeyAName}"/>
                                        <xsl:value-of select="$primaryKey"/>
                                      </td>
                                      <td class="schemaValueColumn">
                                        <xsl:for-each
                                          select="siard:primaryKey/siard:column">
                                          <xsl:variable name="columnAName">
                                            <xsl:call-template name="remove-quotes">
                                              <xsl:with-param name="input" select="."/>
                                            </xsl:call-template>
                                          </xsl:variable>
                                          <xsl:variable name="columnName" select="."/>
                                          <a
                                            href="#{$schemaAName}.{$tableAName}.{$columnAName}">
                                            <xsl:value-of select="$columnName"/>
                                          </a>
                                          <br/>
                                        </xsl:for-each>
                                      </td>
                                      <td class="schemaValueColumn">
                                        <xsl:value-of
                                          select="siard:primaryKey/siard:description"/>
                                      </td>
                                    </tr>
                                  </table>
                                </li>
                              </ul>
                            </xsl:if>
                            <p/>
                            <xsl:if
                              test="count(siard:foreignKeys/siard:foreignKey) &gt; 0">
                              <ul>
                                <li>
                                  <h3 class="small">
                                    Foreign keys
                                  </h3>
                                  <table class="light">
                                    <tr>
                                      <th class="horizontalTitleColumn">
                                        Name
                                      </th>
                                      <th class="horizontalTitleColumn">
                                        Referenced schema
                                      </th>
                                      <th class="horizontalTitleColumn">
                                        Referenced table
                                      </th>
                                      <th class="horizontalTitleColumn">
                                        Reference(s) (Column, Referenced)
                                      </th>
                                      <th class="horizontalTitleColumn">
                                        Match type
                                      </th>
                                      <th class="horizontalTitleColumn">
                                        Delete action
                                      </th>
                                      <th class="horizontalTitleColumn">
                                        Update action
                                      </th>
                                      <th class="horizontalTitleColumn">
                                        Description
                                      </th>
                                    </tr>
                                    <xsl:for-each
                                      select="siard:foreignKeys/siard:foreignKey">
                                      <xsl:variable name="referencedTableAName">
                                        <xsl:call-template name="remove-quotes">
                                          <xsl:with-param name="input"
                                            select="siard:referencedTable"/>
                                        </xsl:call-template>
                                      </xsl:variable>
                                      <xsl:variable name="referencedTable"
                                        select="siard:referencedTable"/>
                                      <xsl:variable name="referencedSchemaAName">
                                        <xsl:call-template name="remove-quotes">
                                          <xsl:with-param name="input"
                                            select="siard:referencedSchema"/>
                                        </xsl:call-template>
                                      </xsl:variable>
                                      <xsl:variable name="referencedSchema"
                                        select="siard:referencedSchema"/>
                                      <xsl:variable name="foreignKeyAName">
                                        <xsl:call-template name="remove-quotes">
                                          <xsl:with-param name="input" select="siard:name"/>
                                        </xsl:call-template>
                                      </xsl:variable>
                                      
                                      <xsl:variable name="foreignKey" select="siard:name"/>
                                      <tr>
                                        <td class="schemaValueColumn">
                                          <a
                                            name="{$schemaAName}.{$tableAName}.{$foreignKeyAName}"/>
                                          <xsl:value-of select="$foreignKey"/>
                                        </td>
                                        <td class="schemaValueColumn">
                                          <a href="#{$referencedSchemaAName}">
                                            <xsl:value-of select="$referencedSchema"/>
                                          </a>
                                        </td>
                                        <td class="schemaValueColumn">
                                          <a
                                            href="#{$referencedSchemaAName}.{$referencedTableAName}">
                                            <xsl:value-of select="$referencedTable"/>
                                          </a>
                                        </td>
                                        <td class="schemaValueColumn">
                                          
                                          <xsl:for-each select="siard:reference">
                                            <xsl:variable name="columnAName">
                                              <xsl:call-template name="remove-quotes">
                                                <xsl:with-param name="input"
                                                  select="siard:column"/>
                                              </xsl:call-template>
                                            </xsl:variable>
                                            <xsl:variable name="columnName"
                                              select="siard:column"/>
                                            <a
                                              href="#{$schemaAName}.{$tableAName}.{$columnAName}">
                                              <xsl:value-of select="$columnName"/>
                                            </a>
                                            ,
                                            <xsl:variable name="referencedColumnAName">
                                              <xsl:call-template name="remove-quotes">
                                                <xsl:with-param name="input"
                                                  select="siard:referenced"/>
                                              </xsl:call-template>
                                            </xsl:variable>
                                            <xsl:variable name="referencedColumn"
                                              select="siard:referenced"/>
                                            <a
                                              href="#{$referencedSchemaAName}.{$referencedTableAName}.{$referencedColumnAName}">
                                              <xsl:value-of select="$referencedColumn"/>
                                            </a>
                                            <br/>
                                          </xsl:for-each>
                                          
                                        </td>
                                        <td class="schemaValueColumn">
                                          <xsl:value-of select="siard:matchType"/>
                                        </td>
                                        <td class="schemaValueColumn">
                                          <xsl:value-of select="siard:deleteAction"/>
                                        </td>
                                        <td class="schemaValueColumn">
                                          <xsl:value-of select="siard:updateAction"/>
                                        </td>
                                        <td class="schemaValueColumn">
                                          <xsl:value-of select="siard:description"/>
                                        </td>
                                      </tr>
                                    </xsl:for-each>
                                  </table>
                                  
                                </li>
                              </ul>
                            </xsl:if>
                            <p/>
                            <xsl:if
                              test="count(siard:candidateKeys/siard:candidateKey) &gt; 0">
                              <ul>
                                <li>
                                  <h3 class="small">
                                    Candidate keys
                                  </h3>
                                  <table class="light">
                                    <tr>
                                      <th class="horizontalTitleColumn">
                                        Name
                                      </th>
                                      <th class="horizontalTitleColumn">
                                        Column(s)
                                      </th>
                                      <th class="horizontalTitleColumn">
                                        Description
                                      </th>
                                    </tr>
                                    <xsl:for-each
                                      select="siard:candidateKeys/siard:candidateKey">
                                      <xsl:variable name="candidateKeyAName">
                                        <xsl:call-template name="remove-quotes">
                                          <xsl:with-param name="input" select="siard:name"/>
                                        </xsl:call-template>
                                      </xsl:variable>
                                      <xsl:variable name="candidateKey"
                                        select="siard:name"/>
                                      <tr>
                                        <td class="schemaValueColumn">
                                          <a
                                            name="{$schemaAName}.{$tableAName}.{$candidateKeyAName}"/>
                                          <xsl:value-of select="$candidateKey"/>
                                        </td>
                                        <td class="schemaValueColumn">
                                          <xsl:for-each select="siard:column">
                                            <xsl:variable name="columnAName">
                                              <xsl:call-template name="remove-quotes">
                                                <xsl:with-param name="input" select="."/>
                                              </xsl:call-template>
                                            </xsl:variable>
                                            <xsl:variable name="columnName" select="."/>
                                            <a
                                              href="#{$schemaAName}.{$tableAName}.{$columnAName}">
                                              <xsl:value-of select="$columnName"/>
                                            </a>
                                            <br/>
                                          </xsl:for-each>
                                          
                                        </td>
                                        <td class="schemaValueColumn">
                                          <xsl:value-of select="siard:description"/>
                                        </td>
                                      </tr>
                                    </xsl:for-each>
                                  </table>
                                  
                                </li>
                              </ul>
                            </xsl:if>
                          </xsl:for-each>
                        </li>
                      </ul>
                    </xsl:if>
                    
                    <xsl:if test="count(siard:views/siard:view) &gt; 0">
                      <ul>
                        <li>
                          <h3 class="small">
                            Views
                          </h3>
                          <xsl:for-each select="siard:views/siard:view">
                            <p/>
                            <xsl:variable name="viewAName">
                              <xsl:call-template name="remove-quotes">
                                <xsl:with-param name="input" select="siard:name"/>
                              </xsl:call-template>
                            </xsl:variable>
                            <xsl:variable name="viewName" select="siard:name"/>
                            
                            <table class="medium">
                              <tr>
                                <td colspan="4" class="tableTitleDark">
                                  VIEW
                                  <a name="{$schemaAName}.{$viewAName}"/>
                                </td>
                              </tr>
                              <tr>
                                <th class="horizontalTitleColumn">
                                  Name
                                </th>
                                <th class="horizontalTitleColumn">
                                  Query
                                </th>
                                <th class="horizontalTitleColumn">
                                  Original query
                                </th>
                                <th class="horizontalTitleColumn">
                                  Description
                                </th>
                              </tr>
                              <tr>
                                <td class="schemaValueColumn">
                                  <xsl:value-of select="$viewName"/>
                                </td>
                                <td class="schemaValueColumn">
                                  <xsl:value-of select="siard:query"/>
                                </td>
                                <td class="schemaValueColumn">
                                  <xsl:value-of select="siard:queryOriginal"/>
                                </td>
                                <td class="schemaValueColumn">
                                  <xsl:value-of select="siard:description"/>
                                </td>
                              </tr>
                            </table>
                            <p/>
                            <xsl:if test="count(siard:columns/siard:column) &gt; 0">
                              <ul>
                                <li>
                                  
                                  <h3 class="small">
                                    Columns
                                  </h3>
                                  
                                  <table class="light">
                                    <tr>
                                      <th class="horizontalTitleColumn">
                                        Name
                                      </th>
                                      <th class="horizontalTitleColumn">
                                        Folder
                                      </th>
                                      <th class="horizontalTitleColumn">
                                        Type
                                      </th>
                                      <th class="horizontalTitleColumn">
                                        Original type
                                      </th>
                                      <th class="horizontalTitleColumn">
                                        Default value
                                      </th>
                                      <th class="horizontalTitleColumn">
                                        Nullable
                                      </th>
                                      <th class="horizontalTitleColumn">
                                        Description
                                      </th>
                                    </tr>
                                    <xsl:for-each select="siard:columns/siard:column">
                                      <tr>
                                        <td class="schemaValueColumn">
                                          <xsl:variable name="columnAName">
                                            <xsl:call-template name="remove-quotes">
                                              <xsl:with-param name="input" select="siard:name"/>
                                            </xsl:call-template>
                                          </xsl:variable>
                                          <xsl:variable name="columnName"
                                            select="siard:name"/>
                                          <xsl:value-of select="$columnName"/>
                                          <a
                                            name="{$schemaAName}.{$viewAName}.{$columnAName}"/>
                                        </td>
                                        <td class="schemaValueColumn">
                                          <xsl:value-of select="siard:folder"/>
                                        </td>
                                        <td class="schemaValueColumn">
                                          <xsl:value-of select="siard:type"/>
                                        </td>
                                        <td class="schemaValueColumn">
                                          <xsl:value-of select="siard:typeOriginal"/>
                                        </td>
                                        <td class="schemaValueColumn">
                                          <xsl:value-of select="siard:defaultValue"/>
                                        </td>
                                        <td class="schemaValueColumn">
                                          <xsl:value-of select="siard:nullable"/>
                                        </td>
                                        <td class="schemaValueColumn">
                                          <xsl:value-of select="siard:description"/>
                                        </td>
                                      </tr>
                                      
                                    </xsl:for-each>
                                  </table>
                                  
                                  
                                </li>
                              </ul>
                            </xsl:if>
                          </xsl:for-each>
                        </li>
                      </ul>
                    </xsl:if>
                    
                    <xsl:if test="count(siard:routines/siard:routine) &gt; 0">
                      <ul>
                        <li>
                          <p/>
                          <h3 class="small">
                            Routines
                          </h3>
                          <xsl:for-each select="siard:routines/siard:routine">
                            <p/>
                            <xsl:variable name="routineAName">
                              <xsl:call-template name="remove-quotes">
                                <xsl:with-param name="input" select="siard:name"/>
                              </xsl:call-template>
                            </xsl:variable>
                            <xsl:variable name="routineName" select="siard:name"/>
                            
                            <table class="medium">
                              <tr>
                                <td colspan="6" class="tableTitleDark">
                                  ROUTINE
                                  <a name="{$schemaAName}.{$routineAName}"/>
                                </td>
                              </tr>
                              <tr>
                                <th class="horizontalTitleColumn">
                                  Name
                                </th>
                                <th class="horizontalTitleColumn">
                                  Description
                                </th>
                                <th class="horizontalTitleColumn">
                                  Source
                                </th>
                                <th class="horizontalTitleColumn">
                                  Body
                                </th>
                                <th class="horizontalTitleColumn">
                                  Characteristic
                                </th>
                                <th class="horizontalTitleColumn">
                                  Return type
                                </th>
                              </tr>
                              <tr>
                                <td class="schemaValueColumn">
                                  <xsl:value-of select="$routineName"/>
                                </td>
                                <td class="schemaValueColumn">
                                  <xsl:value-of select="siard:description"/>
                                </td>
                                <td class="schemaValueColumn">
                                  <xsl:value-of select="siard:source"/>
                                </td>
                                <td class="schemaValueColumn">
                                  <xsl:value-of select="siard:body"/>
                                </td>
                                <td class="schemaValueColumn">
                                  <xsl:value-of select="siard:characteristic"/>
                                </td>
                                <td class="schemaValueColumn">
                                  <xsl:value-of select="siard:returnType"/>
                                </td>
                              </tr>
                            </table>
                            <p/>
                            <xsl:if
                              test="count(siard:parameters/siard:parameter) &gt; 0">
                              <ul>
                                <li>
                                  
                                  <h3 class="small">
                                    Parameters
                                  </h3>
                                  
                                  <table class="light">
                                    <tr>
                                      <th class="horizontalTitleparameter">
                                        Name
                                      </th>
                                      <th class="horizontalTitleparameter">
                                        Mode
                                      </th>
                                      <th class="horizontalTitleparameter">
                                        Type
                                      </th>
                                      <th class="horizontalTitleparameter">
                                        Original type
                                      </th>
                                      <th class="horizontalTitleparameter">
                                        Description
                                      </th>
                                    </tr>
                                    <xsl:for-each
                                      select="siard:parameters/siard:parameter">
                                      <tr>
                                        <td class="schemaValueparameter">
                                          <xsl:variable name="parameterPreAName">
                                            <xsl:call-template name="remove-quotes">
                                              <xsl:with-param name="input" select="siard:name"/>
                                            </xsl:call-template>
                                          </xsl:variable>
                                          <xsl:variable name="parameterAName">
                                            <xsl:call-template name="remove-ats">
                                              <xsl:with-param name="input"
                                                select="$parameterPreAName"/>
                                            </xsl:call-template>
                                          </xsl:variable>
                                          
                                          <xsl:variable name="parameterName"
                                            select="siard:name"/>
                                          <xsl:value-of select="$parameterName"/>
                                          <a
                                            name="{$schemaAName}.{$routineAName}.{$parameterAName}"/>
                                        </td>
                                        <td class="schemaValueparameter">
                                          <xsl:value-of select="siard:mode"/>
                                        </td>
                                        <td class="schemaValueparameter">
                                          <xsl:value-of select="siard:type"/>
                                        </td>
                                        <td class="schemaValueparameter">
                                          <xsl:value-of select="siard:typeOriginal"/>
                                        </td>
                                        <td class="schemaValueparameter">
                                          <xsl:value-of select="siard:description"/>
                                        </td>
                                      </tr>
                                    </xsl:for-each>
                                  </table>
                                  
                                </li>
                              </ul>
                            </xsl:if>
                          </xsl:for-each>
                        </li>
                      </ul>
                    </xsl:if>
                  </li>
                </ul>
              </xsl:for-each>
            </li>
          </ul>
        </xsl:if>
        <!-- Global contents -->
        <h2>
          Schema overall contents
        </h2>
        <xsl:if test="count(siard:users/siard:user) &gt; 0">
          <ul>
            <li>
              <h2 class="small">
                Users
                <a name="users"/>
              </h2>
              
              <table class="strong">
                <tr>
                  <th class="horizontalTitleColumn">
                    Name
                  </th>
                  <th class="horizontalTitleColumn">
                    Description
                  </th>
                </tr>
                <xsl:for-each select="siard:users/siard:user">
                  <xsl:variable name="userAName">
                    <xsl:call-template name="remove-quotes">
                      <xsl:with-param name="input" select="siard:name"/>
                    </xsl:call-template>
                  </xsl:variable>
                  <xsl:variable name="userName" select="siard:name"/>
                  <tr>
                    <td class="userValueColumn">
                      <xsl:value-of select="$userName"/>
                      <a name="user.{$userAName}"/>
                    </td>
                    <td class="userValueColumn">
                      <xsl:value-of select="siard:description"/>
                    </td>
                  </tr>
                </xsl:for-each>
              </table>
              
            </li>
          </ul>
        </xsl:if>
        <xsl:if test="count(siard:roles/siard:role) &gt; 0">
          <ul>
            <li>
              <h2 class="small">
                Roles
                <a name="roles"/>
              </h2>
              
              <table class="strong">
                <tr>
                  <th class="horizontalTitleColumn">
                    Name
                  </th>
                  <th class="horizontalTitleColumn">
                    Administrator
                  </th>
                  <th class="horizontalTitleColumn">
                    Description
                  </th>
                </tr>
                <xsl:for-each select="siard:roles/siard:role">
                  <xsl:variable name="roleName" select="siard:name"/>
                  <xsl:variable name="adminName" select="siard:admin"/>
                  <tr>
                    <td class="userValueColumn">
                      <xsl:value-of select="$roleName"/>
                    </td>
                    <td class="userValueColumn">
                      <a href="#user.{$adminName}">
                        <xsl:value-of select="$adminName"/>
                      </a>
                    </td>
                    
                    <td class="userValueColumn">
                      <xsl:value-of select="siard:description"/>
                    </td>
                  </tr>
                </xsl:for-each>
              </table>
              
            </li>
          </ul>
        </xsl:if>
        <xsl:if test="count(siard:privileges/siard:privilege) &gt; 0">
          <ul>
            <li>
              <h2 class="small">
                Privileges
                <a name="privileges"/>
              </h2>
              
              <table class="strong">
                <tr>
                  <th class="horizontalTitleColumn">
                    Type
                  </th>
                  <th class="horizontalTitleColumn">
                    Object
                  </th>
                  <th class="horizontalTitleColumn">
                    Grantor
                  </th>
                  <th class="horizontalTitleColumn">
                    Grantee
                  </th>
                  <th class="horizontalTitleColumn">
                    Option
                  </th>
                  <th class="horizontalTitleColumn">
                    Description
                  </th>
                </tr>
                <xsl:for-each select="siard:privileges/siard:privilege">
                  <xsl:variable name="objectName" select="siard:object"/>
                  <xsl:variable name="privilegeGrantor" select="siard:grantor"/>
                  <xsl:variable name="privilegeGrantee" select="siard:grantee"/>
                  <tr>
                    <td class="userValueColumn">
                      <xsl:value-of select="siard:type"/>
                    </td>
                    <td class="userValueColumn">
                      
                      <xsl:value-of select="$objectName"/>
                      
                    </td>
                    <td class="userValueColumn">
                      <a href="#user.{$privilegeGrantor}">
                        <xsl:value-of select="$privilegeGrantor"/>
                      </a>
                    </td>
                    <td class="userValueColumn">
                      <a href="#user.{$privilegeGrantee}">
                        <xsl:value-of select="$privilegeGrantee"/>
                      </a>
                    </td>
                    <td class="userValueColumn">
                      <xsl:value-of select="siard:option"/>
                    </td>
                    <td class="userValueColumn">
                      <xsl:value-of select="siard:description"/>
                    </td>
                  </tr>
                </xsl:for-each>
              </table>
              
            </li>
          </ul>
        </xsl:if>
      </body>
    </html>
  </xsl:template>
</xsl:stylesheet>