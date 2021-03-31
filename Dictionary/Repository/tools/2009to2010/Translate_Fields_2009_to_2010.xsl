<?xml version="1.0" encoding="UTF-8"?>

<!-- For making certain fixes to pre-2010 Fields.xml to conform to new format
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

	<xsl:template match="Fields">
		<Field>
			<xsl:call-template name="translateEntityLevelAttributes2009to2010"><xsl:with-param name="cur" select="." /></xsl:call-template>						

			<xsl:copy-of select="Tag" />
			<Name><xsl:value-of select="FieldName" /></Name>
			<xsl:copy-of select="Type" />
			<xsl:if test="LenRefers"><AssociatedDataTag><xsl:value-of select="LenRefers" /></AssociatedDataTag></xsl:if>
			<xsl:copy-of select="AbbrName" />
			<xsl:if test="BaseCatagory"><BaseCategory><xsl:value-of select="BaseCatagory" /></BaseCategory></xsl:if>
			<xsl:if test="BaseCatagoryXMLName"><BaseCategoryAbbrName><xsl:value-of select="BaseCatagoryXMLName" /></BaseCategoryAbbrName></xsl:if>
			<xsl:copy-of select="NotReqXML" />
			<xsl:if test="UsesEnumsFromTag"><EnumDatatype><xsl:value-of select="UsesEnumsFromTag" /></EnumDatatype></xsl:if>
			<xsl:copy-of select="UnionDataType" />
			<Description><xsl:value-of select="Desc" /></Description>
		</Field>
	</xsl:template>			
		
	<xsl:template match="dataroot">
		<Fields copyright="Copyright (c) FIX Protocol Ltd. All Rights Reserved." edition="2010" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"  xsi:noNamespaceSchemaLocation="../../schema/Fields.xsd">
			<xsl:copy-of select="@version" />
			<xsl:copy-of select="@generated" />
			<xsl:if test="@latestEP"><xsl:attribute name="latestEP"><xsl:value-of select="substring(@latestEP,3)" /></xsl:attribute></xsl:if>
			<xsl:apply-templates />				
		</Fields>			
	</xsl:template>		
	<xsl:template match="/">
		<xsl:apply-templates />
	</xsl:template>
</xsl:stylesheet>
