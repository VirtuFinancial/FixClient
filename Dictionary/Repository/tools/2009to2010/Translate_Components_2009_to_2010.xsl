<?xml version="1.0" encoding="UTF-8"?>

<!-- For making certain fixes to pre-2010 Fields.xml to conform to new format
	  This is not a complete transformation. It will not produce a 2010 schema compliant repository file

Revisions
	02-Feb-2010		Phil Oliver

The use of mode is due to difficulty of handling the (stupid) case where MsgType element is re-used in different levels
-->

<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" >
	<xsl:output method="xml" version="1.0" encoding="UTF-8" indent="yes" />

	<xsl:include href="Translate_support.xsl" />

	<xsl:template match="comment()" >
		<xsl:copy/>
	</xsl:template>

	<xsl:template match="processing-instruction()">
	  <xsl:copy/>
	</xsl:template>

	<xsl:template match="Components">
		<Component>
			<xsl:call-template name="translateEntityLevelAttributes2009to2010"><xsl:with-param name="cur" select="." /></xsl:call-template>						

			<ComponentID><xsl:value-of select="MsgID" /></ComponentID>

			<xsl:copy-of select="ComponentType" />

			<CategoryID><xsl:value-of select="Category" /></CategoryID>			

			<Name><xsl:value-of select="ComponentName" /></Name>

			<xsl:if test="AbbrName">
				<xsl:if test="count(OverrideAbbr)=0">
					<AbbrName><xsl:value-of select="AbbrName" /></AbbrName>		
				</xsl:if>
			</xsl:if>
			
			<xsl:if test="OverrideAbbr">
				<AbbrName><xsl:value-of select="OverrideAbbr" /></AbbrName>					
			</xsl:if>
			
			<xsl:copy-of select="NotReqXML" />
			<xsl:copy-of select="Volume" />
			<xsl:copy-of select="Description" />
		</Component>
	</xsl:template>		

	<xsl:template match="dataroot">
		<Components copyright="Copyright (c) FIX Protocol Ltd. All Rights Reserved." edition="2010" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"  xsi:noNamespaceSchemaLocation="../../schema/Sections.xsd">
			<xsl:copy-of select="@version" />
			<xsl:copy-of select="@generated" />
			<xsl:if test="@latestEP"><xsl:attribute name="latestEP"><xsl:value-of select="substring(@latestEP,3)" /></xsl:attribute></xsl:if>
			<xsl:apply-templates />				
		</Components>			
	</xsl:template>	
	
	<xsl:template match="/">
		<xsl:apply-templates />
	</xsl:template>
</xsl:stylesheet>
