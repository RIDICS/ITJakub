<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:xd="http://www.oxygenxml.com/ns/doc/xsl" exclude-result-prefixes="xd" 
	version="2.0">
	<xd:doc scope="stylesheet">
		<xd:desc>
			<xd:p><xd:b>Created on:</xd:b> Dec 4, 2010</xd:p>
			<xd:p><xd:b>Author:</xd:b> boris</xd:p>
			<xd:p/>
		</xd:desc>
	</xd:doc>
	<xsl:output indent="yes"/>

	<xsl:strip-space elements="*"/>

<xsl:include href="Kopirovani_prvku.xsl" />

<xd:doc>
	<xd:desc>
		<xd:p>Pokud tělo obsahuje alespoň jeden element <xd:b>head</xd:b> nebo <xd:b>head1</xd:b> (začínající na "head"), vytvoří se pro takto ohraničené oblasti element <xd:b>div</xd:b>.</xd:p>
		<xd:p>Bylo by vhodné elementy, které předcházejí, rovněž seskupit do <xd:b>div</xd:b>, pokud tyto elementy netvoří <xd:b>div</xd:b> nebo <xd:b>pb</xd:b>.</xd:p>
		<xd:p>Pokud tělo žádný element <xd:b>head</xd:b> nebo <xd:b>head1</xd:b> neobsahuje, elementy se zkopírují. (Nebo by se měla vytvořit alespon jedna úroveň <xd:b>div</xd:b>?)</xd:p>
	</xd:desc>
</xd:doc>
	
	<xsl:template match="/">
		<xsl:comment> Seskupit_prvky_do_div </xsl:comment>
		<xsl:apply-templates />
	</xsl:template>

	<xsl:template match="div[head3]">
		<xsl:copy>
				<xsl:for-each-group select="*" group-starting-with="head3" >
					<div>
						<xsl:for-each select="current-group()">
								<xsl:copy>
									<xsl:copy-of select="@*"></xsl:copy-of>
									<xsl:apply-templates/>
								</xsl:copy>
						</xsl:for-each>
					</div>
				</xsl:for-each-group>
		</xsl:copy>
	</xsl:template>
<!--
	<xsl:template match="body/div[head]">
				<xsl:for-each-group select="*" group-starting-with="head">
					<div>
						<xsl:for-each select="current-group()">
							<xsl:copy>
								<xsl:apply-templates/>
							</xsl:copy>
						</xsl:for-each>
					</div>
				</xsl:for-each-group>
			</xsl:template>

	<xsl:template match="body/div[head3]">
				<xsl:for-each-group select="*" group-starting-with="head3">
					<div>
						<xsl:for-each select="current-group()">
							<xsl:copy>
								<xsl:apply-templates/>
							</xsl:copy>
						</xsl:for-each>
					</div>
					</xsl:for-each-group>-->
	
<!--			<xsl:choose>
				<xsl:when test="count(./child::*[(name() ='head3')]) &gt; 0">
					<xsl:apply-templates select="./*[position() &lt; count(./child::*[(name() ='head3')][1]/preceding-sibling::*)+1]" />
					<xsl:apply-templates select="head3" mode="walker"/>
					<xsl:apply-templates select="./*[position() &gt; count(./child::*[(name() ='head3')][1]/following-sibling::*)+2]"></xsl:apply-templates>
				</xsl:when>
				<xsl:otherwise>
					<xsl:apply-templates />
				</xsl:otherwise>
			</xsl:choose>-->
		
	<!--</xsl:template>-->

<!--<xsl:template match="head3">
	<xsl:apply-templates select="."/>
	<xsl:apply-templates select="head3" mode="walker"/>
</xsl:template>
-->
	
	<!-- /body/child::*/following-sibling::*[name() != 'head1'] -->
<!--	<xsl:template match="preceding-sibling::*/head1[1]" mode="zacatek">
		
		</xsl:template>-->
	
<!--
	
	<xsl:template match="head3" mode="walker">
		<xsl:element name="div">
			<xsl:apply-templates select="."/>
			<xsl:apply-templates select="following-sibling::*[1]" mode="walker"/>
		</xsl:element>
	</xsl:template>

	<xsl:template match="*" mode="walker">
		<xsl:apply-templates select="."/>
		<xsl:if test="not(following-sibling::*[1]/self::head3)">
			<xsl:if test="not(following-sibling::*[1]/self::div)">
				<xsl:apply-templates select="following-sibling::*[1]" mode="walker"/>
			</xsl:if>

		</xsl:if>
	</xsl:template>-->
	

<xsl:template match="div[@type = 'incipit' or @type='explicit']">
	<xsl:copy-of select="."/>
	<!--<xsl:apply-templates />-->
</xsl:template>

</xsl:stylesheet>
