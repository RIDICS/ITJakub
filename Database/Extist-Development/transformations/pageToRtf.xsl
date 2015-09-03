<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns="http://www.w3.org/1999/XSL/Format" xmlns:exist="http://exist.sourceforge.net/NS/exist" xmlns:vw="http://vokabular.ujc.cas.cz/ns/it-jakub/tei/1.0" xmlns:nlp="http://vokabular.ujc.cas.cz/ns/tei-nlp/1.0" xmlns:tei="http://www.tei-c.org/ns/1.0" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:xd="http://www.oxygenxml.com/ns/doc/xsl" xmlns:itj="http://vokabular.ujc.cas.cz/ns/it-jakub/1.0/exist" exclude-result-prefixes="xs xd exist itj vw nlp tei" version="2.0">
    <xsl:import href="xsl-fo/tei-to-xsl-fo-rtf.xsl"/>
    <xd:doc class="style">
        <xd:short>[fo] Set attributes for display of heading for chapters (level 0)</xd:short>
        <xd:detail>&#160;</xd:detail>
    </xd:doc>
    <xsl:template name="setupDiv0">
        <xsl:attribute name="hyphenate">
            <xsl:text>false</xsl:text>
        </xsl:attribute>
        <xsl:attribute name="color">
            <xsl:value-of select="$barvaLoga"/>
        </xsl:attribute>
        <xsl:attribute name="font-size">16pt</xsl:attribute>
        <xsl:attribute name="text-align">left</xsl:attribute>
        <xsl:attribute name="font-weight">bold</xsl:attribute>
        <xsl:attribute name="space-after.optimum">3pt</xsl:attribute>
        <xsl:attribute name="space-after.maximum">6pt</xsl:attribute>
        <xsl:attribute name="space-before.optimum">9pt</xsl:attribute>
        <xsl:attribute name="space-before.maximum">12pt</xsl:attribute>
        <xsl:attribute name="text-indent">
            <xsl:value-of select="$headingOutdent"/>
        </xsl:attribute>
        <xsl:attribute name="keep-with-next.within-page">always</xsl:attribute>
        <xsl:if test="not(parent::vw:fragment)">
            <xsl:attribute name="page-break-before">always</xsl:attribute>
        </xsl:if>
        <xsl:if test="@xml:id">
            <xsl:attribute name="id">
                <xsl:value-of select="@xml:id"/>
            </xsl:attribute>
        </xsl:if>
    </xsl:template>
    <xsl:template match="tei:pb">
        <xsl:variable name="e">
            <xsl:choose>
                <xsl:when test="parent::vw:fragment">
                    <xsl:text>block</xsl:text>
                </xsl:when>
                <xsl:when test="parent::tei:body or parent::tei:front or      parent::tei:back or parent::tei:div">
                    <xsl:text>block</xsl:text>
                </xsl:when>
                <xsl:otherwise>
                    <xsl:text>inline</xsl:text>
                </xsl:otherwise>
            </xsl:choose>
        </xsl:variable>
        <xsl:choose>
            <xsl:when test="parent::tei:list"/>
            <xsl:when test="$pagebreakStyle='active'">
                <xsl:element name="{$e}">
                    <xsl:attribute name="break-before">page</xsl:attribute>
                    <xsl:if test="@xml:id">
                        <xsl:attribute name="id">
                            <xsl:value-of select="@xml:id"/>
                        </xsl:attribute>
                    </xsl:if>
                </xsl:element>
            </xsl:when>
            <xsl:when test="$pagebreakStyle='visible'">
                <xsl:element name="{$e}">
                    <xsl:if test="@xml:id">
                        <xsl:attribute name="id">
                            <xsl:value-of select="@xml:id"/>
                        </xsl:attribute>
                    </xsl:if>
                    <xsl:attribute name="font-weight">bold</xsl:attribute>
                    <xsl:attribute name="font-size">
                        <xsl:value-of select="$bodySize"/>
                    </xsl:attribute>
                    <xsl:attribute name="color">#000000</xsl:attribute>
                    <xsl:attribute name="keep-together.within-line">
                        <xsl:value-of select="'always'"/>
                    </xsl:attribute>
                    <xsl:text>[</xsl:text>
                    <xsl:value-of select="@n"/>
                    <xsl:text>]</xsl:text>
                    <xsl:if test="@rend = 'space'">
                        <xsl:text> </xsl:text>
                    </xsl:if>
                    <xsl:if test="$e eq 'block' and following-sibling::*[1]/self::tei:note">
                        <xsl:apply-templates select="following-sibling::*[1]" mode="top-element"/>
                    </xsl:if>
                </xsl:element>
            </xsl:when>
        </xsl:choose>
    </xsl:template>
    <xd:doc>
        <xd:short>Process elements  tei:note properly</xd:short>
        <xd:detail>&#160;</xd:detail>
    </xd:doc>
    <xsl:template match="tei:note" mode="top-element">
        <xsl:choose>
            <xsl:when test="@place='end'">
                <simple-link>
                    <xsl:attribute name="internal-destination">
                        <xsl:value-of select="generate-id()"/>
                    </xsl:attribute>
                    <inline font-size="{$footnotenumSize}" vertical-align="super">
                        <xsl:choose>
                            <xsl:when test="@n">
                                <xsl:value-of select="@n"/>
                            </xsl:when>
                            <xsl:otherwise>
                                <xsl:call-template name="calculateEndNoteNumber"/>
                            </xsl:otherwise>
                        </xsl:choose>
                    </inline>
                </simple-link>
            </xsl:when>
            <xsl:when test="@place='inline'">
                <inline>
                    <xsl:text> (</xsl:text>
                    <xsl:apply-templates/>
                    <xsl:text>)</xsl:text>
                </inline>
            </xsl:when>
            <xsl:when test="@place='display'">
                <block text-indent="0pt" end-indent="{$exampleMargin}" start-indent="{$exampleMargin}" font-size="{$exampleSize}" space-before.optimum="{$exampleBefore}" space-after.optimum="{$exampleAfter}">
                    <xsl:apply-templates/>
                </block>
            </xsl:when>
            <xsl:when test="@place='divtop'">
                <block text-indent="0pt" end-indent="{$exampleMargin}" start-indent="{$exampleMargin}" font-style="italic" font-size="{$exampleSize}" space-before.optimum="{$exampleBefore}" space-after.optimum="{$exampleAfter}">
                    <xsl:apply-templates/>
                </block>
            </xsl:when>
            <xsl:otherwise>
                <xsl:call-template name="makeFootnote"/>
            </xsl:otherwise>
        </xsl:choose>
    </xsl:template>
    <xd:doc>
        <xd:short>Process elements  tei:note properly</xd:short>
        <xd:detail>&#160;</xd:detail>
    </xd:doc>
    <xsl:template match="tei:note" mode="real">
        <xsl:choose>
            <xsl:when test="parent::vw:fragment or parent::tei:body">
				
            </xsl:when>
            <xsl:when test="@place='end'">
                <simple-link>
                    <xsl:attribute name="internal-destination">
                        <xsl:value-of select="generate-id()"/>
                    </xsl:attribute>
                    <inline font-size="{$footnotenumSize}" vertical-align="super">
                        <xsl:choose>
                            <xsl:when test="@n">
                                <xsl:value-of select="@n"/>
                            </xsl:when>
                            <xsl:otherwise>
                                <xsl:call-template name="calculateEndNoteNumber"/>
                            </xsl:otherwise>
                        </xsl:choose>
                    </inline>
                </simple-link>
            </xsl:when>
            <xsl:when test="@place='inline'">
                <inline>
                    <xsl:text> (</xsl:text>
                    <xsl:apply-templates/>
                    <xsl:text>)</xsl:text>
                </inline>
            </xsl:when>
            <xsl:when test="@place='display'">
                <block text-indent="0pt" end-indent="{$exampleMargin}" start-indent="{$exampleMargin}" font-size="{$exampleSize}" space-before.optimum="{$exampleBefore}" space-after.optimum="{$exampleAfter}">
                    <xsl:apply-templates/>
                </block>
            </xsl:when>
            <xsl:when test="@place='divtop'">
                <block text-indent="0pt" end-indent="{$exampleMargin}" start-indent="{$exampleMargin}" font-style="italic" font-size="{$exampleSize}" space-before.optimum="{$exampleBefore}" space-after.optimum="{$exampleAfter}">
                    <xsl:apply-templates/>
                </block>
            </xsl:when>
            <xsl:otherwise>
                <xsl:call-template name="makeFootnote"/>
            </xsl:otherwise>
        </xsl:choose>
    </xsl:template>
</xsl:stylesheet>