<?xml version="1.0" encoding="UTF-8"?>

<!-- 

Converts 2010 edition file to 2009 compatible file

Revisions
	19-Jun-2010		Phil Oliver
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

	<xsl:template match="Categories">
		<Category>
			<xsl:call-template name="translateEntityLevelAttributes2009to2010"><xsl:with-param name="cur" select="." /></xsl:call-template>						

			<CategoryID><xsl:value-of select="Category" /></CategoryID>
			<xsl:copy-of select="FIXMLFileName" />
			<xsl:copy-of select="NotReqXML" />			
			<xsl:copy-of select="GenerateImplFile" />	
			<xsl:copy-of select="ComponentType" />
			<xsl:choose>
				<xsl:when test="Volume='2'"><SectionID>Session</SectionID></xsl:when>
				<xsl:when test="Volume='3'"><SectionID>PreTrade</SectionID></xsl:when>
				<xsl:when test="Volume='4'"><SectionID>Trade</SectionID></xsl:when>
				<xsl:when test="Volume='5'"><SectionID>PostTrade</SectionID></xsl:when>									
			</xsl:choose>
			<xsl:copy-of select="Volume" />
			<xsl:copy-of select="IncludeFile" />			
		</Category>
	</xsl:template>		

	<xsl:template match="dataroot">
		<Categories copyright="Copyright (c) FIX Protocol Ltd. All Rights Reserved." edition="2010" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"  xsi:noNamespaceSchemaLocation="../../schema/Categories.xsd">
			<xsl:copy-of select="@version" />
			<xsl:copy-of select="@generated" />
			<xsl:if test="@latestEP"><xsl:attribute name="latestEP"><xsl:value-of select="substring(@latestEP,3)" /></xsl:attribute></xsl:if>
			<xsl:apply-templates />				
		</Categories>
	</xsl:template>	
	
	<xsl:template match="/">
		<xsl:apply-templates />
	</xsl:template>
</xsl:stylesheet>
