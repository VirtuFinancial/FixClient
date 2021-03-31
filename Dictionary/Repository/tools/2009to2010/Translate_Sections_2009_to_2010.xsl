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

	<xsl:template match="Sections">
		<Section>
			<xsl:call-template name="translateEntityLevelAttributes2009to2010"><xsl:with-param name="cur" select="." /></xsl:call-template>						

			<SectionID><xsl:value-of select="DisplayName" /></SectionID>
			<Name><xsl:value-of select="DisplayName" /></Name>			
			<xsl:copy-of select="DisplayOrder" />
			<xsl:copy-of select="Volume" />			
			<xsl:copy-of select="NotReqXML" />
			<xsl:copy-of select="FIXMLFileName" />
			<Description><xsl:value-of select="Desc" /></Description>
		</Section>
	</xsl:template>		

	<xsl:template match="dataroot">
		<Sections copyright="Copyright (c) FIX Protocol Ltd. All Rights Reserved." edition="2009" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"  xsi:noNamespaceSchemaLocation="../../schema/Sections.xsd">
			<xsl:copy-of select="@version" />
			<xsl:copy-of select="@generated" />
			<xsl:if test="@latestEP"><xsl:attribute name="latestEP"><xsl:value-of select="substring(@latestEP,3)" /></xsl:attribute></xsl:if>
			<xsl:apply-templates />				
		</Sections>
	</xsl:template>	
	
	<xsl:template match="/">
		<xsl:apply-templates />
	</xsl:template>
</xsl:stylesheet>
