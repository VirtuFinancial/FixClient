<?xml version="1.0" encoding="UTF-8"?>

<!-- For making certain fixes to pre-2010 Enums.xml to conform to new format
	  This is not a complete transformation. It will not produce a 2010 schema compliant repository file

Revisions
	02-Feb-2010		Phil Oliver
-->

<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:output method="xml" version="1.0" encoding="UTF-8" indent="yes"/>
		
	<xsl:include href="Translate_support.xsl" />

	<xsl:template match="comment()" >
		<xsl:copy/>
	</xsl:template>

	<xsl:template match="processing-instruction()">
	  <xsl:copy/>
	</xsl:template>

	<xsl:template match="Enums">
		<Enum>
			<xsl:call-template name="translateEntityLevelAttributes2009to2010"><xsl:with-param name="cur" select="." /></xsl:call-template>						

			<xsl:copy-of select="Tag" />
			<Value><xsl:value-of select="Enum" /></Value>
			<xsl:copy-of select="SymbolicName" />
			<Group><xsl:value-of select="Group" /></Group>
			<xsl:copy-of select="Sort" />
			<xsl:copy-of select="Description" />
		</Enum>
	</xsl:template>			
	
	<xsl:template match="dataroot">
		<Enums copyright="Copyright (c) FIX Protocol Ltd. All Rights Reserved." edition="2010" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"  xsi:noNamespaceSchemaLocation="../../schema/Enums.xsd">
			<xsl:copy-of select="@version" />
			<xsl:copy-of select="@generated" />
			<xsl:if test="@latestEP"><xsl:attribute name="latestEP"><xsl:value-of select="substring(@latestEP,3)" /></xsl:attribute></xsl:if>
			<xsl:apply-templates />				
		</Enums>			
	</xsl:template>	
	
	<xsl:template match="/">
		<xsl:apply-templates />
	</xsl:template>
</xsl:stylesheet>
