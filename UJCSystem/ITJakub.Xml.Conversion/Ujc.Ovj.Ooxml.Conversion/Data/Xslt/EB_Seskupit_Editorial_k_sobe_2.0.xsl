<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet exclude-result-prefixes="xs xd" version="2.0" xmlns:xd="http://www.oxygenxml.com/ns/doc/xsl" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
    <xd:doc scope="stylesheet">
        <xd:desc>
            <xd:p><xd:b>Created on:</xd:b> Feb 22, 2012</xd:p>
            <xd:p><xd:b>Author:</xd:b> Boris</xd:p>
            <xd:p />
        </xd:desc>
    </xd:doc>

    <xsl:include href="Kopirovani_prvku.xsl" />
    <xsl:output indent="yes" />
    <xsl:strip-space elements="*" />


    <xsl:template match="/">
        <xsl:comment> EB_Seskupit_Editorial_k_sobe_2.0 </xsl:comment>
        <xsl:apply-templates />
    </xsl:template>

    <xsl:template match="body">
        <xsl:copy>
        	<xsl:apply-templates mode="seskupit" select="div[@type='editorial' and @subtype='title']" />
        	<xsl:apply-templates mode="seskupit" select="div[@type='editorial' and @subtype='annotation']" />
        	<div type="editorial" subtype="comment">
        		<xsl:apply-templates mode="seskupit" select="div[@type='editorial' and @subtype='comment'] | thead[@type='editorial'] | head[@type='editorial'] | head1[@type='editorial']" />
        	</div>
        	<xsl:apply-templates />
        </xsl:copy>
    </xsl:template>

	<xsl:template match="div[@type='editorial']" mode="seskupit">
		<xsl:apply-templates />
	</xsl:template>
	
	<xsl:template match="div[@type='editorial' and @subtype='grant'] | div[@type='editorial' and @subtype='title'] | div[@type='editorial' and @subtype='annotation']" mode="seskupit"  priority="10">
		<xsl:copy>
			<xsl:copy-of select="@*" />
		<xsl:apply-templates />
		</xsl:copy>
	</xsl:template>

	<xsl:template match="thead[@type='editorial'] | head[@type='editorial'] | head1[@type='editorial']" mode="seskupit">
        <xsl:copy>
            <xsl:copy-of select="@*" />
            <xsl:apply-templates />
        </xsl:copy>
    </xsl:template>

	<xsl:template match="div[@type='editorial'and @subtype='comment'] | thead[@type='editorial'] | head[@type='editorial'] | head1[@type='editorial']" />
	<xsl:template match="div[@type='editorial'and @subtype='title'] | div[@type='editorial'and @subtype='annotation']" />
	
</xsl:stylesheet>
