<?xml version="1.0" encoding="UTF-8"?>

<!-- 
    Converts 2009 MsgType.xml to 2010 Messages.xml

Revisions
	02-Feb-2010		Phil Oliver
    20-Jun-2010         "      "
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

	<xsl:template match="MsgType">
		<Message>
			<xsl:call-template name="translateEntityLevelAttributes2009to2010"><xsl:with-param name="cur" select="." /></xsl:call-template>						

			<ComponentID><xsl:value-of select="MsgID" /></ComponentID>
			<xsl:copy-of select="MsgType" />
			<Name><xsl:value-of select="MessageName" /></Name>
			<CategoryID><xsl:value-of select="Category" /></CategoryID>
			<xsl:choose>
				<xsl:when test="Section='Pre Trade'"><SectionID>PreTrade</SectionID></xsl:when>
				<xsl:when test="Section='Post Trade'"><SectionID>PostTrade</SectionID></xsl:when>
				<xsl:otherwise><SectionID><xsl:value-of select="Section" /></SectionID></xsl:otherwise>
			</xsl:choose>
			<xsl:copy-of select="AbbrName" />
			<xsl:copy-of select="NotReqXML" />
			<xsl:copy-of select="Description" />
		</Message>
	</xsl:template>			

	<xsl:template match="dataroot">
		<Messages copyright="Copyright (c) FIX Protocol Ltd. All Rights Reserved." edition="2010" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"  xsi:noNamespaceSchemaLocation="../../schema/Messages.xsd">
			<xsl:copy-of select="@version" />
			<xsl:copy-of select="@generated" />
			<xsl:if test="@latestEP"><xsl:attribute name="latestEP"><xsl:value-of select="substring(@latestEP,3)" /></xsl:attribute></xsl:if>
			<xsl:apply-templates />				
		</Messages>			
	</xsl:template>
	
	<xsl:template match="/">
		<xsl:apply-templates />
	</xsl:template>
</xsl:stylesheet>
