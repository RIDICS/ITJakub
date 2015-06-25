<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns="http://www.w3.org/1999/XSL/Format" xmlns:nlp="http://vokabular.ujc.cas.cz/ns/tei-nlp/1.0" xmlns:tei="http://www.tei-c.org/ns/1.0" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:xd="http://www.oxygenxml.com/ns/doc/xsl" exclude-result-prefixes="xs xd nlp tei" version="2.0">
	<?itj-book-type Edition?>
	<?itj-output-format Rtf?>	
    <xsl:import href="fo2/tei.xsl"/>
    <xsl:import href="ujc-ovj-rozmery-a4.xsl"/>
    <xsl:import href="ujc-ovj-xsl-fo.xsl"/>
    <xd:doc scope="stylesheet">
        <xd:desc>
            <xd:p>
                <xd:b>Created on:</xd:b> Jun 19, 2015</xd:p>
            <xd:p>
                <xd:b>Author:</xd:b> lehecka</xd:p>
            <xd:p>Slouží ke generování XSL-FO pro transformaci na dokument ve formátu RFT</xd:p>
        </xd:desc>
    </xd:doc>
    <xd:doc class="style" type="string">Font size for footnote numbers</xd:doc>
    <xsl:param name="footnotenumSize">9pt</xsl:param>
	
    <xsl:param name="titlePage">true</xsl:param>
    <xsl:param name="divRunningheads">false</xsl:param>
    <xsl:template match="tei:pb"/>
    <xd:doc>
        <xd:short>Process elements tei:front</xd:short>
        <xd:detail>&#160;</xd:detail>
    </xd:doc>
    <xsl:template match="tei:front">
        <xsl:comment>Front matter</xsl:comment>
        <xsl:choose>
            <xsl:when test="ancestor::tei:floatingText">
                <xsl:apply-templates/>
            </xsl:when>
            <xsl:when test="ancestor::tei:p">
                <xsl:apply-templates/>
            </xsl:when>
            <xsl:when test="ancestor::tei:group">
                <xsl:apply-templates/>
            </xsl:when>
            <xsl:otherwise>
				
            </xsl:otherwise>
        </xsl:choose>
    </xsl:template>
    <xsl:template name="pageMasterHook">
        <simple-page-master master-name="obalka" page-width="{$pageWidth}" page-height="{$pageHeight}" margin-top="0pt" margin-bottom="0pt" margin-left="0pt" margin-right="0pt">
            <region-body column-count="{$columnCount}" margin-bottom="0pt" margin-top="0pt"/>
            <region-before region-name="xsl-region-before-first" extent="0pt"/>
            <region-after region-name="xsl-region-after-first" extent="0pt"/>
        </simple-page-master>
        <simple-page-master master-name="uvod1" page-width="{$pageWidth}" page-height="{$pageHeight}" margin-top="{$pageMarginTop}" margin-bottom="{$pageMarginBottom}" margin-left="{$pageMarginLeft}" margin-right="{$pageMarginRight}">
            <region-body margin-bottom="{$bodyMarginBottom}" margin-top="{$bodyMarginTop}"/>
            <region-before extent="{$regionBeforeExtent}"/>
            <region-after extent="{$regionAfterExtent}"/>
        </simple-page-master>
        <page-sequence-master master-name="zacatek1">
            <repeatable-page-master-alternatives>
                <conditional-page-master-reference master-reference="obalka" page-position="first"/>
                <conditional-page-master-reference master-reference="uvod1"/>
            </repeatable-page-master-alternatives>
        </page-sequence-master>
    </xsl:template>
</xsl:stylesheet>