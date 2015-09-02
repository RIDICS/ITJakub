<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns="http://www.w3.org/1999/XSL/Format" xmlns:exist="http://exist.sourceforge.net/NS/exist" xmlns:vw="http://vokabular.ujc.cas.cz/ns/it-jakub/tei/1.0" xmlns:nlp="http://vokabular.ujc.cas.cz/ns/tei-nlp/1.0" xmlns:tei="http://www.tei-c.org/ns/1.0" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:xd="http://www.oxygenxml.com/ns/doc/xsl" xmlns:itj="http://vokabular.ujc.cas.cz/ns/it-jakub/1.0/exist" exclude-result-prefixes="xs xd exist itj vw nlp tei" version="2.0">
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
    <xsl:template match="itj:result">
        <root>
            <xsl:call-template name="setupPagemasters"/>
            <xsl:call-template name="mainAction"/>
        </root>
    </xsl:template>
    <xd:doc>
        <xd:short>Process elements  tei:body</xd:short>
        <xd:detail>&#160;</xd:detail>
    </xd:doc>
    <xsl:template match="vw:fragment">
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
				
                <page-sequence format="{$formatBodypage}" text-align="{$alignment}" hyphenate="{$hyphenate}" language="{$language}" initial-page-number="1">
                    <xsl:call-template name="choosePageMaster">
                        <xsl:with-param name="where">
                            <xsl:value-of select="$bodyMulticolumns"/>
                        </xsl:with-param>
                    </xsl:call-template>
					
                    <xsl:choose>
                        <xsl:when test="$twoSided='true'">
                            <xsl:call-template name="headers-footers-twoside"/>
                        </xsl:when>
                        <xsl:otherwise>
                            <xsl:call-template name="headers-footers-oneside"/>
                        </xsl:otherwise>
                    </xsl:choose>
					
                    <flow flow-name="xsl-region-body" font-family="{$bodyFont}" font-size="{$bodySize}">
                        <xsl:if test="not($flowMarginLeft='')">
                            <xsl:attribute name="margin-left">
                                <xsl:value-of select="$flowMarginLeft"/>
                            </xsl:attribute>
                        </xsl:if>
						
                        <xsl:if test="not($titlePage='true') and not(preceding-sibling::tei:front)">
                            <xsl:call-template name="Header"/>
                        </xsl:if>
                        <xsl:apply-templates/>
                        <xsl:if test=".//tei:note[@place='end']">
                            <block>
                                <xsl:call-template name="setupDiv2"/>
                                <xsl:text>Notes</xsl:text>
                            </block>
                            <xsl:apply-templates select=".//tei:note[@place='end']" mode="endnote"/>
                        </xsl:if>
                    </flow>
                </page-sequence>
            </xsl:otherwise>
        </xsl:choose>
    </xsl:template>
    <xsl:template name="mainAction">
        <xsl:choose>
            <xsl:when test="tei:text/tei:group">
                <xsl:apply-templates select="tei:text/tei:front"/>
                <xsl:apply-templates select="tei:text/tei:group"/>
            </xsl:when>
            <xsl:otherwise>
                <xsl:apply-templates select="tei:text/tei:front"/>
                <xsl:apply-templates select="tei:text/tei:body"/>
            </xsl:otherwise>
        </xsl:choose>
        <xsl:apply-templates select="tei:text/tei:back"/>
        <xsl:apply-templates select="vw:fragment"/>
    </xsl:template>
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