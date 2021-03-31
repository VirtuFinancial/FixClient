<?xml version="1.0" encoding="UTF-8"?>

<!-- 

Converts 2010 edition file to 2009 compatible file

Revisions
	01-Mar-2010		Phil Oliver
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

	<xsl:template match="Abbreviations">
		<Abbreviation>
			<xsl:call-template name="translateEntityLevelAttributes2009to2010"><xsl:with-param name="cur" select="." /></xsl:call-template>						

			<xsl:copy-of select="Term" />

			<AbbrTerm><xsl:value-of select="Abbreviation" /></AbbrTerm>

			<xsl:copy-of select="Usage" />
		</Abbreviation>
	</xsl:template>		

	<xsl:template match="dataroot">
		<Abbreviations copyright="Copyright (c) FIX Protocol Ltd. All Rights Reserved." edition="2010" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"  xsi:noNamespaceSchemaLocation="../../schema/Abbreviations.xsd">
			<xsl:copy-of select="@version" />
			<xsl:copy-of select="@generated" />
			<xsl:if test="@latestEP"><xsl:attribute name="latestEP"><xsl:value-of select="substring(@latestEP,3)" /></xsl:attribute></xsl:if>
			<xsl:apply-templates />				
		</Abbreviations>
	</xsl:template>	
	
	<xsl:template match="/">
		<xsl:apply-templates />
	</xsl:template>
</xsl:stylesheet>
