<xsl:stylesheet xmlns="http://www.w3.org/1999/XSL/Format" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:a="http://relaxng.org/ns/compatibility/annotations/1.0" xmlns:teix="http://www.tei-c.org/ns/Examples" xmlns:tei="http://www.tei-c.org/ns/1.0" xmlns:rng="http://relaxng.org/ns/structure/1.0" xmlns:xd="http://www.pnp-software.com/XSLTdoc" xmlns:fotex="http://www.tug.org/fotex" exclude-result-prefixes="xd a fotex rng tei teix" version="2.0">
    <xd:doc type="stylesheet">
        <xd:short>
    TEI stylesheet
    dealing  with elements from the
      core module, making XSL-FO output.
      </xd:short>
        <xd:detail>
    This library is free software; you can redistribute it and/or
    modify it under the terms of the GNU Lesser General Public
    License as published by the Free Software Foundation; either
    version 2.1 of the License, or (at your option) any later version.

    This library is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
    Lesser General Public License for more details.

    You should have received a copy of the GNU Lesser General Public
    License along with this library; if not, write to the Free Software
    Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA

   
   
      </xd:detail>
        <xd:author>See AUTHORS</xd:author>
        <xd:cvsId>$Id: core.xsl 6982 2009-11-12 22:02:50Z rahtz $</xd:cvsId>
        <xd:copyright>2008, TEI Consortium</xd:copyright>
    </xd:doc>
    <xd:doc>
        <xd:short>Process elements  processing-instruction()[name()='xmltex']</xd:short>
        <xd:detail>&#160;</xd:detail>
    </xd:doc>
    <xsl:template match="processing-instruction()[name()='xmltex']">
        <xsl:message>xmltex pi <xsl:value-of select="."/>
        </xsl:message>
        <xsl:copy-of select="."/>
    </xsl:template>
    <xd:doc>
        <xd:short>Process elements  tei:ab</xd:short>
        <xd:detail>&#160;</xd:detail>
    </xd:doc>
    <xsl:template match="tei:ab">
        <block>
            <xsl:apply-templates/>
        </block>
    </xsl:template>
    <xd:doc>
        <xd:short>Process elements  tei:abbr</xd:short>
        <xd:detail>&#160;</xd:detail>
    </xd:doc>
    <xsl:template match="tei:abbr">
        <xsl:apply-templates/>
    </xsl:template>
    <xd:doc>
        <xd:short>Process elements  tei:add</xd:short>
        <xd:detail>&#160;</xd:detail>
    </xd:doc>
    <xsl:template match="tei:add">
        <xsl:choose>
            <xsl:when test="@place='sup'">
                <inline vertical-align="super">
                    <xsl:apply-templates/>
                </inline>
            </xsl:when>
            <xsl:when test="@place='sub'">
                <inline vertical-align="sub">
                    <xsl:apply-templates/>
                </inline>
            </xsl:when>
            <xsl:otherwise>
                <xsl:apply-templates/>
            </xsl:otherwise>
        </xsl:choose>
    </xsl:template>
    <xd:doc>
        <xd:short>Process elements  tei:byline</xd:short>
        <xd:detail>&#160;</xd:detail>
    </xd:doc>
    <xsl:template match="tei:byline">
        <block text-align="center">
            <xsl:apply-templates/>
        </block>
    </xsl:template>
    <xd:doc>
        <xd:short>Process elements  tei:cell//tei:lb</xd:short>
        <xd:detail>&#160;</xd:detail>
    </xd:doc>
    <xsl:template match="tei:cell//tei:lb">
        <xsl:choose>
            <xsl:when test="$foEngine='passivetex'"> </xsl:when>
            <xsl:otherwise>
                <inline linefeed-treatment="preserve">
                    <xsl:text>
</xsl:text>
                </inline>
            </xsl:otherwise>
        </xsl:choose>
    </xsl:template>
    <xd:doc>
        <xd:short>Process elements  tei:code</xd:short>
        <xd:detail>&#160;</xd:detail>
    </xd:doc>
    <xsl:template match="tei:code">
        <inline font-family="{$typewriterFont}">
            <xsl:apply-templates/>
        </inline>
    </xsl:template>
    <xd:doc>
        <xd:short>Process elements  tei:corr</xd:short>
        <xd:detail>&#160;</xd:detail>
    </xd:doc>
    <xsl:template match="tei:corr">
        <xsl:text>[</xsl:text>
        <xsl:apply-templates/>
        <xsl:text>]</xsl:text>
        <xsl:choose>
            <xsl:when test="@sic">
                <footnote>
                    <footnote-citation>
                        <inline font-size="8pt" vertical-align="super">
                            <xsl:number format="a" level="any" count="tei:corr"/>
                        </inline>
                    </footnote-citation>
                    <list-block>
                        <xsl:attribute name="provisional-distance-between-starts">
                            <xsl:value-of select="$betweenStarts"/>
                        </xsl:attribute>
                        <xsl:attribute name="provisional-label-separation">
                            <xsl:value-of select="$labelSeparation"/>
                        </xsl:attribute>
                        <list-item>
                            <list-item-label end-indent="label-end()">
                                <block>
                                    <inline font-size="{$footnoteSize}" vertical-align="super">
                                        <xsl:number format="a" level="any" count="tei:corr"/>
                                    </inline>
                                </block>
                            </list-item-label>
                            <list-item-body start-indent="body-start()">
                                <block font-size="{$footnoteSize}">
                                    <xsl:value-of select="@sic"/>
                                </block>
                            </list-item-body>
                        </list-item>
                    </list-block>
                </footnote>
            </xsl:when>
        </xsl:choose>
    </xsl:template>
    <xd:doc>
        <xd:short>Process elements  tei:del</xd:short>
        <xd:detail>&#160;</xd:detail>
    </xd:doc>
    <xsl:template match="tei:del">
        <inline text-decoration="line-through">
            <xsl:apply-templates/>
        </inline>
    </xsl:template>
    <xd:doc>
        <xd:short>Process elements  tei:eg</xd:short>
        <xd:detail>&#160;</xd:detail>
    </xd:doc>
    <xsl:template match="tei:eg">
        <block font-family="{$typewriterFont}" background-color="{$exampleBackgroundColor}" color="{$exampleColor}" white-space-treatment="preserve" linefeed-treatment="preserve" white-space-collapse="false" wrap-option="no-wrap" text-indent="0em" hyphenate="false" start-indent="{$exampleMargin}" text-align="start" font-size="{$exampleSize}" space-before.optimum="4pt" space-after.optimum="4pt">
            <xsl:if test="not($flowMarginLeft='')">
                <xsl:attribute name="padding-start">
                    <xsl:value-of select="$exampleMargin"/>
                </xsl:attribute>
            </xsl:if>
            <xsl:if test="parent::tei:exemplum">
                <xsl:text>
</xsl:text>
            </xsl:if>
            <xsl:value-of select="translate(.,' ','&#160;')"/>
        </block>
    </xsl:template>
    <xd:doc>
        <xd:short>Process elements  tei:eg</xd:short>
        <xd:detail>&#160;</xd:detail>
    </xd:doc>
    <xsl:template match="teix:egXML">
        <xsl:param name="simple">false</xsl:param>
        <xsl:param name="highlight"/>
        <block font-family="{$typewriterFont}" background-color="{$exampleBackgroundColor}" color="{$exampleColor}" white-space-treatment="preserve" linefeed-treatment="preserve" white-space-collapse="false" wrap-option="no-wrap" text-indent="0em" hyphenate="false" start-indent="{$exampleMargin}" text-align="start" font-size="{$exampleSize}" space-before.optimum="4pt" space-after.optimum="4pt">
            <xsl:if test="not($flowMarginLeft='')">
                <xsl:attribute name="padding-start">
                    <xsl:value-of select="$exampleMargin"/>
                </xsl:attribute>
            </xsl:if>
            <xsl:apply-templates mode="verbatim">
                <xsl:with-param name="highlight">
                    <xsl:value-of select="$highlight"/>
                </xsl:with-param>
            </xsl:apply-templates>
        </block>
    </xsl:template>
    <xd:doc>
        <xd:short>Process elements  tei:eg[@rend='kwic']/lb</xd:short>
        <xd:detail>&#160;</xd:detail>
    </xd:doc>
    <xsl:template match="tei:eg[@rend='kwic']/lb"/>
    <xd:doc>
        <xd:short>Process elements  tei:emph</xd:short>
        <xd:detail>&#160;</xd:detail>
    </xd:doc>
    <xsl:template match="tei:emph">
        <inline font-style="italic">
            <xsl:apply-templates/>
        </inline>
    </xsl:template>
    <xd:doc>
        <xd:short>Process elements  tei:epigraph</xd:short>
        <xd:detail>&#160;</xd:detail>
    </xd:doc>
    <xsl:template match="tei:epigraph">
        <block text-align="center" space-before.optimum="4pt" space-after.optimum="4pt" start-indent="{$exampleMargin}">
            <xsl:apply-templates/>
        </block>
    </xsl:template>
    <xd:doc>
        <xd:short>Process elements  tei:epigraph/tei:lg</xd:short>
        <xd:detail>&#160;</xd:detail>
    </xd:doc>
    <xsl:template match="tei:epigraph/tei:lg">
        <block text-align="center" space-before.optimum="4pt" space-after.optimum="4pt">
            <xsl:apply-templates/>
        </block>
    </xsl:template>
    <xd:doc>
        <xd:short>Process elements  tei:epigraph/tei:q</xd:short>
        <xd:detail>&#160;</xd:detail>
    </xd:doc>
    <xsl:template match="tei:epigraph/tei:q">
        <block space-before.optimum="4pt" space-after.optimum="4pt" start-indent="{$exampleMargin}">
            <xsl:apply-templates/>
        </block>
    </xsl:template>
    <xd:doc>
        <xd:short>Process elements  tei:foreign</xd:short>
        <xd:detail>&#160;</xd:detail>
    </xd:doc>
    <xsl:template match="tei:foreign">
        <inline font-style="italic">
            <xsl:apply-templates/>
        </inline>
    </xsl:template>
    <xd:doc>
        <xd:short>Process elements  tei:gap</xd:short>
        <xd:detail>&#160;</xd:detail>
    </xd:doc>
    <xsl:template match="tei:gap">
        <inline border-style="solid">
            <xsl:text>[</xsl:text>
            <xsl:value-of select="@reason"/>
            <xsl:text>]</xsl:text>
        </inline>
    </xsl:template>
    <xd:doc>
        <xd:short>Process elements  tei:gi</xd:short>
        <xd:detail>&#160;</xd:detail>
    </xd:doc>
    <xsl:template match="tei:gi">
        <inline hyphenate="false" color="{$giColor}" font-family="{$typewriterFont}">
            <xsl:text>&lt;</xsl:text>
            <xsl:apply-templates/>
            <xsl:text>&gt;</xsl:text>
        </inline>
    </xsl:template>
    <xd:doc>
        <xd:short>Process elements  tei:att</xd:short>
        <xd:detail>&#160;</xd:detail>
    </xd:doc>
    <xsl:template match="tei:att">
        <inline hyphenate="false" color="{$giColor}" font-family="{$typewriterFont}" font-weight="bold">
            <xsl:text>@</xsl:text>
            <xsl:apply-templates/>
        </inline>
    </xsl:template>
    <xd:doc>
        <xd:short>Process elements  tei:gloss</xd:short>
        <xd:detail>&#160;</xd:detail>
    </xd:doc>
    <xsl:template match="tei:gloss">
        <xsl:apply-templates/>
    </xsl:template>
    <xd:doc>
        <xd:short>Process elements  tei:hi</xd:short>
        <xd:detail>&#160;</xd:detail>
    </xd:doc>
    <xsl:template match="tei:hi">
        <inline>
            <xsl:call-template name="rend">
                <xsl:with-param name="defaultvalue" select="string('bold')"/>
                <xsl:with-param name="defaultstyle" select="string('font-weight')"/>
                <xsl:with-param name="rend" select="@rend"/>
            </xsl:call-template>
            <xsl:apply-templates/>
        </inline>
    </xsl:template>
    <xd:doc>
        <xd:short>Process elements  tei:ident</xd:short>
        <xd:detail>&#160;</xd:detail>
    </xd:doc>
    <xsl:template match="tei:ident">
        <inline color="{$identColor}" font-family="{$sansFont}">
            <xsl:apply-templates/>
        </inline>
    </xsl:template>
    <xd:doc>
        <xd:short>Process elements  tei:index</xd:short>
        <xd:detail>&#160;</xd:detail>
    </xd:doc>
    <xsl:template match="tei:index">
        <xsl:apply-templates/>
    </xsl:template>
    <xd:doc>
        <xd:short>Process elements  tei:interp</xd:short>
        <xd:detail>&#160;</xd:detail>
    </xd:doc>
    <xsl:template match="tei:interp">
        <xsl:apply-templates/>
    </xsl:template>
    <xd:doc>
        <xd:short>Process elements  tei:interpGrp</xd:short>
        <xd:detail>&#160;</xd:detail>
    </xd:doc>
    <xsl:template match="tei:interpGrp">
        <xsl:apply-templates/>
    </xsl:template>
    <xd:doc>
        <xd:short>Process elements  tei:item</xd:short>
        <xd:detail>&#160;</xd:detail>
    </xd:doc>
    <xsl:template match="tei:item" mode="catalogue">
        <table-cell>
            <block>
                <xsl:choose>
                    <xsl:when test="tei:label">
                        <inline font-weight="bold">
                            <xsl:apply-templates select="tei:label" mode="print"/>
                        </inline>
                    </xsl:when>
                    <xsl:otherwise>
                        <inline font-weight="bold">
                            <xsl:apply-templates mode="print" select="preceding-sibling::tei:*[1]"/>
                        </inline>
                    </xsl:otherwise>
                </xsl:choose>
            </block>
        </table-cell>
        <table-cell>
            <block>
                <xsl:apply-templates/>
            </block>
        </table-cell>
    </xsl:template>
    <xd:doc>
        <xd:short>Process elements  tei:item</xd:short>
        <xd:detail>&#160;</xd:detail>
    </xd:doc>
    <xsl:template match="tei:item|tei:biblStruct">
        <xsl:call-template name="makeItem"/>
    </xsl:template>
    <xsl:template match="tei:item" mode="xref">
        <xsl:variable name="listdepth" select="count(ancestor::tei:list)"/>
        <xsl:if test="parent::tei:list[@type='bibliography']">
            <xsl:text> [</xsl:text>
        </xsl:if>
        <xsl:variable name="listNFormat">
            <xsl:choose>
                <xsl:when test="$listdepth=1">
                    <xsl:text>1</xsl:text>
                </xsl:when>
                <xsl:when test="$listdepth=2">
                    <xsl:text>i</xsl:text>
                </xsl:when>
                <xsl:when test="$listdepth=3">
                    <xsl:text>a</xsl:text>
                </xsl:when>
                <xsl:when test="$listdepth=4">
                    <xsl:text>I</xsl:text>
                </xsl:when>
            </xsl:choose>
        </xsl:variable>
        <xsl:number format="{$listNFormat}"/>
        <xsl:if test="parent::tei:list[@type='bibliography']">
            <xsl:text>]</xsl:text>
        </xsl:if>
    </xsl:template>
    <xd:doc>
        <xd:short>Process elements  tei:kw</xd:short>
        <xd:detail>&#160;</xd:detail>
    </xd:doc>
    <xsl:template match="tei:kw">
        <inline font-style="italic">
            <xsl:apply-templates/>
        </inline>
    </xsl:template>
    <xd:doc>
        <xd:short>Process elements  tei:l</xd:short>
        <xd:detail>&#160;</xd:detail>
    </xd:doc>
    <xsl:template match="tei:l">
        <block space-before.optimum="0pt" space-after.optimum="0pt">
            <xsl:choose>
                <xsl:when test="starts-with(@rend,'indent(')">
                    <xsl:attribute name="text-indent">
                        <xsl:value-of select="concat(substring-before(substring-after(@rend,'('),')'),'em')"/>
                    </xsl:attribute>
                </xsl:when>
                <xsl:when test="starts-with(@rend,'indent')">
                    <xsl:attribute name="text-indent">1em</xsl:attribute>
                </xsl:when>
            </xsl:choose>
            <xsl:apply-templates/>
        </block>
    </xsl:template>
    <xd:doc>
        <xd:short>Process elements  tei:label</xd:short>
        <xd:detail>&#160;</xd:detail>
    </xd:doc>
    <xsl:template match="tei:label" mode="print">
        <xsl:apply-templates/>
    </xsl:template>
    <xd:doc>
        <xd:short>Process elements  tei:label</xd:short>
        <xd:detail>&#160;</xd:detail>
    </xd:doc>
    <xsl:template match="tei:label"/>
    <xd:doc>
        <xd:short>Process elements  tei:lb</xd:short>
        <xd:detail>&#160;</xd:detail>
    </xd:doc>
    <xsl:template match="tei:lb">
        <xsl:choose>
            <xsl:when test="$activeLinebreaks='true'">
                <xsl:choose>
                    <xsl:when test="$foEngine='passivetex'"> </xsl:when>
                    <xsl:when test="parent::tei:list">
                        <list-item>
                            <list-item-label>
                                <block/>
                            </list-item-label>
                            <list-item-body>
                                <block/>
                            </list-item-body>
                        </list-item>
                    </xsl:when>
                    <xsl:otherwise>
                        <block/>
                    </xsl:otherwise>
                </xsl:choose>
            </xsl:when>
            <xsl:otherwise>
                <inline font-size="8pt">
                    <xsl:text>❡</xsl:text>
                </inline>
            </xsl:otherwise>
        </xsl:choose>
    </xsl:template>
    <xd:doc>
        <xd:short>Process elements  tei:list</xd:short>
        <xd:detail>&#160;</xd:detail>
    </xd:doc>
    <xsl:template match="tei:list|tei:listBibl">
        <xsl:if test="child::tei:head">
            <block font-style="italic" text-align="start" space-before.optimum="4pt">
                <xsl:for-each select="tei:head">
                    <xsl:apply-templates/>
                </xsl:for-each>
            </block>
        </xsl:if>
        <xsl:choose>
            <xsl:when test="@type='runin'">
                <block>
                    <xsl:apply-templates mode="runin"/>
                </block>
            </xsl:when>
            <xsl:otherwise>
                <list-block>
                    <xsl:call-template name="setListIndents"/>
                    <xsl:apply-templates/>
                </list-block>
            </xsl:otherwise>
        </xsl:choose>
    </xsl:template>
    <xd:doc>
        <xd:short>Process elements  tei:listBibl/tei:bibl</xd:short>
        <xd:detail>&#160;</xd:detail>
    </xd:doc>
    <xsl:template match="tei:listBibl/tei:bibl">
        <xsl:call-template name="makeItem"/>
    </xsl:template>
    <xd:doc>
        <xd:short>Process elements  tei:list[@type='catalogue']</xd:short>
        <xd:detail>&#160;</xd:detail>
    </xd:doc>
    <xsl:template match="tei:list[@type='catalogue']">
        <block space-before="{$spaceAroundTable}" space-after="{$spaceAroundTable}">
            <table>
                <table-column column-number="1" column-width="20%">
                    <xsl:if test="$foEngine='passivetex'">
                        <xsl:attribute name="column-align" namespace="http://www.tug.org/fotex">p</xsl:attribute>
                    </xsl:if>
                </table-column>
                <table-column column-number="2" column-width="80%">
                    <xsl:if test="$foEngine='passivetex'">
                        <xsl:attribute name="column-align" namespace="http://www.tug.org/fotex">p</xsl:attribute>
                    </xsl:if>
                </table-column>
                <table-body>
                    <xsl:for-each select="tei:item">
                        <table-row>
                            <xsl:apply-templates select="." mode="catalogue"/>
                        </table-row>
                    </xsl:for-each>
                </table-body>
            </table>
        </block>
    </xsl:template>
    <xd:doc>
        <xd:short>Process elements  tei:lg</xd:short>
        <xd:detail>&#160;</xd:detail>
    </xd:doc>
    <xsl:template match="tei:lg">
        <block start-indent="{$exampleMargin}" text-align="start" space-before.optimum="4pt" space-after.optimum="4pt">
            <xsl:apply-templates/>
        </block>
    </xsl:template>
    <xd:doc>
        <xd:short>Process elements  tei:mentioned</xd:short>
        <xd:detail>&#160;</xd:detail>
    </xd:doc>
    <xsl:template match="tei:mentioned">
        <inline>
            <xsl:call-template name="rend">
                <xsl:with-param name="defaultvalue" select="string('italic')"/>
                <xsl:with-param name="defaultstyle" select="string('font-style')"/>
            </xsl:call-template>
            <xsl:apply-templates/>
        </inline>
    </xsl:template>
    <xd:doc>
        <xd:short>Process elements  tei:milestone</xd:short>
        <xd:detail>&#160;</xd:detail>
    </xd:doc>
    <xsl:template match="tei:milestone">
        <block>
            <xsl:text>******************</xsl:text>
            <xsl:value-of select="@unit"/>
            <xsl:text> </xsl:text>
            <xsl:value-of select="@n"/>
            <xsl:text>******************</xsl:text>
        </block>
    </xsl:template>
    <xd:doc>
        <xd:short>Process elements  tei:name</xd:short>
        <xd:detail>&#160;</xd:detail>
    </xd:doc>
    <xsl:template match="tei:name">
        <xsl:apply-templates/>
    </xsl:template>
    <xd:doc>
        <xd:short>Process elements  tei:note (endnote mode)</xd:short>
        <xd:detail>&#160;</xd:detail>
    </xd:doc>
    <xsl:template match="tei:note" mode="endnote">
        <block id="{generate-id()}">
            <xsl:call-template name="calculateEndNoteNumber"/>
            <xsl:text>. </xsl:text>
            <xsl:apply-templates/>
        </block>
    </xsl:template>
    <xd:doc>
        <xd:short>Process elements  tei:note</xd:short>
        <xd:detail>&#160;</xd:detail>
    </xd:doc>
    <xsl:template match="tei:note">
        <xsl:choose>
            <xsl:when test="ancestor::tei:p or ancestor::tei:item">
                <xsl:apply-templates select="." mode="real"/>
            </xsl:when>
            <xsl:otherwise>
                <block>
                    <xsl:apply-templates select="." mode="real"/>
                </block>
            </xsl:otherwise>
        </xsl:choose>
    </xsl:template>
    <xd:doc>
        <xd:short>Process elements  tei:note properly</xd:short>
        <xd:detail>&#160;</xd:detail>
    </xd:doc>
    <xsl:template match="tei:note" mode="real">
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
                <xsl:choose>
                    <xsl:when test="parent::tei:item">
                        <block>
                            <xsl:call-template name="makeFootnote"/>
                        </block>
                    </xsl:when>
                    <xsl:otherwise>
                        <xsl:call-template name="makeFootnote"/>
                    </xsl:otherwise>
                </xsl:choose>
            </xsl:otherwise>
        </xsl:choose>
    </xsl:template>
    <xd:doc>
        <xd:short>Create a footnote</xd:short>
        <xd:detail>&#160;</xd:detail>
    </xd:doc>
    <xsl:template name="makeFootnote">
        <xsl:variable name="FootID">
            <xsl:choose>
                <xsl:when test="@n">
                    <xsl:value-of select="@n"/>
                </xsl:when>
                <xsl:otherwise>
                    <xsl:call-template name="calculateFootnoteNumber"/>
                </xsl:otherwise>
            </xsl:choose>
        </xsl:variable>
        <footnote>
            <inline>
                <xsl:if test="not(@target)">
                    <xsl:attribute name="font-size">
                        <xsl:value-of select="$footnotenumSize"/>
                    </xsl:attribute>
                    <xsl:attribute name="vertical-align">super</xsl:attribute>
                    <xsl:value-of select="$FootID"/>
                </xsl:if>
            </inline>
            <footnote-body>
                <block end-indent="0pt" start-indent="0pt" text-align="start" font-style="normal" text-indent="{$parIndent}" font-size="{$footnoteSize}">
                    <xsl:if test="@xml:id">
                        <xsl:attribute name="id">
                            <xsl:value-of select="@xml:id"/>
                        </xsl:attribute>
                    </xsl:if>
                    <xsl:if test="not(@target)">
                        <inline font-size="{$footnotenumSize}" vertical-align="super">
                            <xsl:value-of select="$FootID"/>
                        </inline>
                        <xsl:text> </xsl:text>
                    </xsl:if>
                    <xsl:apply-templates/>
                </block>
            </footnote-body>
        </footnote>
    </xsl:template>
    <xd:doc>
        <xd:short>Process element  tei:p</xd:short>
        <xd:detail>&#160;</xd:detail>
    </xd:doc>
    <xsl:template match="tei:p">
        <block>
            <xsl:if test="preceding-sibling::tei:p">
                <xsl:attribute name="text-indent">
                    <xsl:value-of select="$parIndent"/>
                </xsl:attribute>
                <xsl:attribute name="space-before.optimum">
                    <xsl:value-of select="$parSkip"/>
                </xsl:attribute>
                <xsl:attribute name="space-before.maximum">
                    <xsl:value-of select="$parSkipmax"/>
                </xsl:attribute>
            </xsl:if>
            <xsl:if test="@xml:lang">
                <xsl:attribute name="country">
                    <xsl:value-of select="substring-before(@xml:lang,'-')"/>
                </xsl:attribute>
                <xsl:attribute name="language">
                    <xsl:value-of select="substring-after(@xml:lang,'-')"/>
                </xsl:attribute>
            </xsl:if>
            <xsl:apply-templates/>
        </block>
    </xsl:template>
    <xd:doc>
        <xd:short>Process elements  tei:pb</xd:short>
        <xd:detail>&#160;</xd:detail>
    </xd:doc>
    <xsl:template match="tei:pb">
        <xsl:variable name="e">
            <xsl:choose>
                <xsl:when test="parent::tei:body or parent::tei:front or    parent::tei:back or parent::tei:div">
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
                    <xsl:text>✁[</xsl:text>
                    <xsl:value-of select="@unit"/>
                    <xsl:text> Page </xsl:text>
                    <xsl:value-of select="@n"/>
                    <xsl:text>]✁</xsl:text>
                </xsl:element>
            </xsl:when>
        </xsl:choose>
    </xsl:template>
    <xd:doc>
        <xd:short>Process elements  tei:quote</xd:short>
        <xd:detail>&#160;</xd:detail>
    </xd:doc>
    <xsl:template match="tei:quote">
        <block text-align="start" text-indent="0pt" end-indent="{$exampleMargin}" start-indent="{$exampleMargin}" font-size="{$exampleSize}" space-before.optimum="{$exampleBefore}" space-after.optimum="{$exampleAfter}">
            <xsl:apply-templates/>
        </block>
    </xsl:template>
    <xd:doc>
        <xd:short>Process elements  tei:q</xd:short>
        <xd:detail>&#160;</xd:detail>
    </xd:doc>
    <xsl:template match="tei:q">
        <xsl:choose>
            <xsl:when test="tei:text">
                <xsl:apply-templates/>
            </xsl:when>
            <xsl:when test="@rend='display' or tei:p or tei:lg">
                <block text-align="start" text-indent="0pt" end-indent="{$exampleMargin}" start-indent="{$exampleMargin}" font-size="{$exampleSize}" space-before.optimum="{$exampleBefore}" space-after.optimum="{$exampleAfter}">
                    <xsl:apply-templates/>
                </block>
            </xsl:when>
            <xsl:when test="@rend='eg'">
                <block text-align="start" font-size="{$exampleSize}" space-before.optimum="4pt" text-indent="0pt" space-after.optimum="4pt" start-indent="{$exampleMargin}" font-family="{$typewriterFont}">
                    <xsl:apply-templates/>
                </block>
            </xsl:when>
            <xsl:when test="@rend = 'qwic'">
                <block space-before="{$spaceAroundTable}" space-after="{$spaceAroundTable}">
                    <inline-container>
                        <table font-size="{$exampleSize}" font-family="{$typewriterFont}" start-indent="{$exampleMargin}">
                            <table-column column-number="1" column-width="">
                                <xsl:if test="$foEngine='passivetex'">
                                    <xsl:attribute name="column-align" namespace="http://www.tug.org/fotex">p</xsl:attribute>
                                </xsl:if>
                            </table-column>
                            <table-column column-number="2" column-width="">
                                <xsl:if test="$foEngine='passivetex'">
                                    <xsl:attribute name="column-align" namespace="http://www.tug.org/fotex">l</xsl:attribute>
                                </xsl:if>
                            </table-column>
                            <table-body>
                                <xsl:for-each select="tei:q">
                                    <xsl:for-each select="tei:term">
                                        <table-row>
                                            <table-cell>
                                                <block>
                                                    <xsl:apply-templates select="preceding-sibling::node()"/>
                                                </block>
                                            </table-cell>
                                            <table-cell>
                                                <block>
                                                    <xsl:apply-templates/>
                                                    <xsl:apply-templates select="following-sibling::node()"/>
                                                </block>
                                            </table-cell>
                                        </table-row>
                                    </xsl:for-each>
                                </xsl:for-each>
                            </table-body>
                        </table>
                    </inline-container>
                </block>
            </xsl:when>
            <xsl:when test="starts-with(@rend,'kwic')">
                <block space-before="{$spaceAroundTable}" space-after="{$spaceAroundTable}">
                    <inline-container>
                        <table font-size="{$exampleSize}" start-indent="{$exampleMargin}" font-family="{$typewriterFont}">
                            <table-column column-number="1" column-width="">
                                <xsl:if test="$foEngine='passivetex'">
                                    <xsl:attribute name="column-align" namespace="http://www.tug.org/fotex">r</xsl:attribute>
                                </xsl:if>
                            </table-column>
                            <table-column column-number="2" column-width="">
                                <xsl:if test="$foEngine='passivetex'">
                                    <xsl:attribute name="column-align" namespace="http://www.tug.org/fotex">l</xsl:attribute>
                                </xsl:if>
                            </table-column>
                            <table-body>
                                <xsl:for-each select="tei:term">
                                    <table-row>
                                        <table-cell>
                                            <block>
                                                <xsl:value-of select="preceding-sibling::node()[1]"/>
                                            </block>
                                        </table-cell>
                                        <table-cell>
                                            <block>
                                                <xsl:apply-templates/>
                                                <xsl:value-of select="following-sibling::node()[1]"/>
                                            </block>
                                        </table-cell>
                                    </table-row>
                                </xsl:for-each>
                            </table-body>
                        </table>
                    </inline-container>
                </block>
            </xsl:when>
            <xsl:when test="@rend='literal'">
                <block white-space-collapse="false" wrap-option="no-wrap" font-size="{$exampleSize}" space-before.optimum="4pt" space-after.optimum="4pt" start-indent="{$exampleMargin}" font-family="{$typewriterFont}">
                    <xsl:apply-templates/>
                </block>
            </xsl:when>
            <xsl:otherwise>
                <xsl:text>“</xsl:text>
                <xsl:apply-templates/>
                <xsl:text>”</xsl:text>
            </xsl:otherwise>
        </xsl:choose>
    </xsl:template>
    <xd:doc>
        <xd:short>Process elements  tei:reg</xd:short>
        <xd:detail>&#160;</xd:detail>
    </xd:doc>
    <xsl:template match="tei:reg">
        <inline font-family="{$sansFont}">
            <xsl:apply-templates/>
        </inline>
    </xsl:template>
    <xd:doc>
        <xd:short>Process elements  tei:rs</xd:short>
        <xd:detail>&#160;</xd:detail>
    </xd:doc>
    <xsl:template match="tei:rs">
        <xsl:apply-templates/>
    </xsl:template>
    <xd:doc>
        <xd:short>Process elements  tei:s</xd:short>
        <xd:detail>&#160;</xd:detail>
    </xd:doc>
    <xsl:template match="tei:s">
        <xsl:apply-templates/>
    </xsl:template>
    <xd:doc>
        <xd:short>Process elements  tei:salute</xd:short>
        <xd:detail>&#160;</xd:detail>
    </xd:doc>
    <xsl:template match="tei:salute">
        <block text-align="left">
            <xsl:apply-templates/>
        </block>
    </xsl:template>
    <xd:doc>
        <xd:short>Process elements  tei:seg</xd:short>
        <xd:detail>&#160;</xd:detail>
    </xd:doc>
    <xsl:template match="tei:seg">
        <block font-family="{$typewriterFont}" background-color="yellow" white-space-collapse="false" wrap-option="no-wrap" text-indent="0em" start-indent="{$exampleMargin}" text-align="start" font-size="{$exampleSize}" padding-before="8pt" padding-after="8pt" space-before.optimum="4pt" space-after.optimum="4pt">
            <xsl:apply-templates/>
        </block>
    </xsl:template>
    <xd:doc>
        <xd:short>Process elements  tei:sic</xd:short>
        <xd:detail>&#160;</xd:detail>
    </xd:doc>
    <xsl:template match="tei:sic">
        <xsl:apply-templates/>
        <xsl:text> (sic)</xsl:text>
    </xsl:template>
    <xd:doc>
        <xd:short>Process elements  tei:signed</xd:short>
        <xd:detail>&#160;</xd:detail>
    </xd:doc>
    <xsl:template match="tei:signed">
        <block text-align="left">
            <xsl:apply-templates/>
        </block>
    </xsl:template>
    <xd:doc>
        <xd:short>Process elements  tei:term</xd:short>
        <xd:detail>&#160;</xd:detail>
    </xd:doc>
    <xsl:template match="tei:term">
        <inline font-style="italic">
            <xsl:apply-templates/>
        </inline>
    </xsl:template>
    <xd:doc>
        <xd:short>Process elements  tei:unclear</xd:short>
        <xd:detail>&#160;</xd:detail>
    </xd:doc>
    <xsl:template match="tei:unclear">
        <inline text-decoration="blink">
            <xsl:apply-templates/>
        </inline>
    </xsl:template>
    <xd:doc>
        <xd:short>[fo] </xd:short>
        <xd:detail>&#160;</xd:detail>
    </xd:doc>
    <xsl:template name="addID">
        <xsl:attribute name="id">
            <xsl:choose>
                <xsl:when test="@xml:id">
                    <xsl:value-of select="@xml:id"/>
                </xsl:when>
                <xsl:otherwise>
                    <xsl:value-of select="generate-id()"/>
                </xsl:otherwise>
            </xsl:choose>
        </xsl:attribute>
    </xsl:template>
    <xd:doc>
        <xd:short>[fo] process "rend" attribute</xd:short>
        <xd:param name="value">value of "rend" attribute</xd:param>
        <xd:detail>&#160;</xd:detail>
    </xd:doc>
    <xsl:template name="applyRend">
        <xsl:param name="value"/>
        <xsl:choose>
            <xsl:when test="$value='gothic'">
                <xsl:attribute name="font-family">fantasy</xsl:attribute>
            </xsl:when>
            <xsl:when test="$value='calligraphic'">
                <xsl:attribute name="font-family">cursive</xsl:attribute>
            </xsl:when>
            <xsl:when test="$value='ital' or $value='italic' or $value='it' or $value='i' or $value='italics'">
                <xsl:attribute name="font-style">italic</xsl:attribute>
            </xsl:when>
            <xsl:when test="$value='sc'">
                <xsl:attribute name="font-variant">small-caps</xsl:attribute>
            </xsl:when>
            <xsl:when test="$value='code'">
                <xsl:attribute name="font-family">
                    <xsl:value-of select="$typewriterFont"/>
                </xsl:attribute>
            </xsl:when>
            <xsl:when test="$value='bo' or $value='bold'">
                <xsl:attribute name="font-weight">bold</xsl:attribute>
            </xsl:when>
            <xsl:when test="$value='BO'">
                <xsl:attribute name="font-style">italic</xsl:attribute>
                <xsl:attribute name="text-decoration">underline</xsl:attribute>
            </xsl:when>
            <xsl:when test="$value='UL' or $value='ul'">
                <xsl:attribute name="text-decoration">underline</xsl:attribute>
            </xsl:when>
            <xsl:when test="$value='sub'">
                <xsl:attribute name="vertical-align">sub</xsl:attribute>
            </xsl:when>
            <xsl:when test="$value='small'">
                <xsl:attribute name="font-size">small</xsl:attribute>
            </xsl:when>
            <xsl:when test="$value='strike'">
                <xsl:attribute name="text-decoration">line-through</xsl:attribute>
            </xsl:when>
            <xsl:when test="$value='sup'">
                <xsl:attribute name="vertical-align">super</xsl:attribute>
            </xsl:when>
        </xsl:choose>
    </xsl:template>
    <xd:doc>
        <xd:short>[fo] </xd:short>
        <xd:detail>&#160;</xd:detail>
    </xd:doc>
    <xsl:template name="calculateEndNoteNumber">
        <xsl:number level="any" format="i" count="tei:note[@place='end']"/>
    </xsl:template>
    <xd:doc>
        <xd:short>[fo] </xd:short>
        <xd:detail>&#160;</xd:detail>
    </xd:doc>
    <xsl:template name="calculateFootnoteNumber">
        <xsl:number from="tei:text" level="any" count="tei:note"/>
    </xsl:template>
    <xd:doc>
        <xd:short>[fo] </xd:short>
        <xd:detail>&#160;</xd:detail>
    </xd:doc>
    <xsl:template name="makeItem">
        <xsl:variable name="listdepth" select="count(ancestor::tei:list)"/>
        <list-item>
            <xsl:if test="not(parent::tei:note[@place='foot' or @place='bottom' ])">
                <xsl:attribute name="space-before.optimum">
                    <xsl:value-of select="$listItemsep"/>
                </xsl:attribute>
            </xsl:if>
            <list-item-label end-indent="label-end()">
                <xsl:if test="@xml:id">
                    <xsl:attribute name="id">
                        <xsl:value-of select="@xml:id"/>
                    </xsl:attribute>
                </xsl:if>
                <xsl:text>
</xsl:text>
                <block>
                    <xsl:choose>
                        <xsl:when test="@n">
                            <xsl:attribute name="text-align">end</xsl:attribute>
                            <xsl:value-of select="@n"/>
                        </xsl:when>
                        <xsl:when test="../@type='bibliography'">
                            <xsl:attribute name="text-align">end</xsl:attribute>
                            <xsl:apply-templates mode="xref" select="."/>
                        </xsl:when>
                        <xsl:when test="../@type='ordered' or self::tei:bibl">
                            <xsl:attribute name="text-align">end</xsl:attribute>
                            <xsl:apply-templates mode="xref" select="."/>
                            <xsl:text>.</xsl:text>
                        </xsl:when>
                        <xsl:when test="../@type='gloss' or self::tei:biblStruct">
                            <xsl:attribute name="text-align">start</xsl:attribute>
                            <xsl:attribute name="font-weight">bold</xsl:attribute>
                            <xsl:choose>
                                <xsl:when test="self::tei:biblStruct">
                                    <xsl:apply-templates mode="xref" select="."/>
                                </xsl:when>
                                <xsl:when test="tei:label">
                                    <xsl:apply-templates mode="print" select="tei:label"/>
                                </xsl:when>
                                <xsl:otherwise>
                                    <xsl:apply-templates mode="print" select="preceding-sibling::tei:*[1]"/>
                                </xsl:otherwise>
                            </xsl:choose>
                        </xsl:when>
                        <xsl:when test="../@type='numbered'">
                            <xsl:attribute name="text-align">end</xsl:attribute>
                            <xsl:number/>
                            <xsl:text>.</xsl:text>
                        </xsl:when>
                        <xsl:when test="../@type='ordered'">
                            <xsl:attribute name="text-align">end</xsl:attribute>
                            <xsl:number/>
                            <xsl:text>.</xsl:text>
                        </xsl:when>
                        <xsl:otherwise>
                            <xsl:attribute name="text-align">end</xsl:attribute>
                            <xsl:choose>
                                <xsl:when test="$listdepth=0">
                                    <xsl:value-of select="$bulletOne"/>
                                </xsl:when>
                                <xsl:when test="$listdepth=1">
                                    <xsl:value-of select="$bulletOne"/>
                                </xsl:when>
                                <xsl:when test="$listdepth=2">
                                    <xsl:value-of select="$bulletTwo"/>
                                </xsl:when>
                                <xsl:when test="$listdepth=3">
                                    <xsl:value-of select="$bulletThree"/>
                                </xsl:when>
                                <xsl:when test="$listdepth=4">
                                    <xsl:value-of select="$bulletFour"/>
                                </xsl:when>
                            </xsl:choose>
                        </xsl:otherwise>
                    </xsl:choose>
                </block>
            </list-item-label>
            <list-item-body start-indent="body-start()">
                <xsl:choose>
                    <xsl:when test="*">
                        <xsl:for-each select="*">
                            <xsl:choose>
                                <xsl:when test="self::tei:list">
                                    <xsl:apply-templates select="."/>
                                </xsl:when>
                                <xsl:otherwise>
                                    <block font-weight="normal">
                                        <xsl:apply-templates/>
                                    </block>
                                </xsl:otherwise>
                            </xsl:choose>
                        </xsl:for-each>
                    </xsl:when>
                    <xsl:otherwise>
                        <block font-weight="normal">
                            <xsl:apply-templates/>
                        </block>
                    </xsl:otherwise>
                </xsl:choose>
            </list-item-body>
        </list-item>
    </xsl:template>
    <xd:doc>
        <xd:short>[fo] </xd:short>
        <xd:param name="defaultvalue">defaultvalue</xd:param>
        <xd:param name="defaultstyle">defaultstyle</xd:param>
        <xd:param name="rend">rend</xd:param>
        <xd:detail>&#160;</xd:detail>
    </xd:doc>
    <xsl:template name="rend">
        <xsl:param name="defaultvalue"/>
        <xsl:param name="defaultstyle"/>
        <xsl:param name="rend"/>
        <xsl:choose>
            <xsl:when test="$rend=''">
                <xsl:attribute name="{$defaultstyle}">
                    <xsl:value-of select="$defaultvalue"/>
                </xsl:attribute>
            </xsl:when>
            <xsl:when test="contains($rend,';')">
                <xsl:call-template name="applyRend">
                    <xsl:with-param name="value" select="substring-before($rend,';')"/>
                </xsl:call-template>
                <xsl:call-template name="rend">
                    <xsl:with-param name="rend" select="substring-after($rend,';')"/>
                </xsl:call-template>
            </xsl:when>
            <xsl:otherwise>
                <xsl:call-template name="applyRend">
                    <xsl:with-param name="value" select="$rend"/>
                </xsl:call-template>
            </xsl:otherwise>
        </xsl:choose>
    </xsl:template>
    <xd:doc>
        <xd:short>[fo] Spacing setup for list blocks</xd:short>
        <xd:detail>&#160;</xd:detail>
    </xd:doc>
    <xsl:template name="setListIndents">
        <xsl:attribute name="provisional-distance-between-starts">
            <xsl:choose>
                <xsl:when test="self::tei:listBibl[tei:biblStruct]">
                    <xsl:value-of select="$betweenBiblStarts"/>
                </xsl:when>
                <xsl:when test="@type='gloss'">
                    <xsl:value-of select="$betweenGlossStarts"/>
                </xsl:when>
                <xsl:otherwise>
                    <xsl:value-of select="$betweenStarts"/>
                </xsl:otherwise>
            </xsl:choose>
        </xsl:attribute>
        <xsl:attribute name="provisional-label-separation">
            <xsl:value-of select="$labelSeparation"/>
        </xsl:attribute>
        <xsl:attribute name="text-indent">0em</xsl:attribute>
        <xsl:attribute name="margin-right">
            <xsl:value-of select="$listRightMargin"/>
        </xsl:attribute>
        <xsl:variable name="listdepth" select="count(ancestor::tei:list)"/>
        <xsl:choose>
            <xsl:when test="$listdepth=0">
                <xsl:attribute name="space-before">
                    <xsl:value-of select="$listAbove-1"/>
                </xsl:attribute>
                <xsl:attribute name="space-after">
                    <xsl:value-of select="$listBelow-1"/>
                </xsl:attribute>
            </xsl:when>
            <xsl:when test="$listdepth=1">
                <xsl:attribute name="space-before">
                    <xsl:value-of select="$listAbove-2"/>
                </xsl:attribute>
                <xsl:attribute name="space-after">
                    <xsl:value-of select="$listBelow-2"/>
                </xsl:attribute>
            </xsl:when>
            <xsl:when test="$listdepth=2">
                <xsl:attribute name="space-before">
                    <xsl:value-of select="$listAbove-3"/>
                </xsl:attribute>
                <xsl:attribute name="space-after">
                    <xsl:value-of select="$listBelow-3"/>
                </xsl:attribute>
            </xsl:when>
            <xsl:when test="$listdepth=3">
                <xsl:attribute name="space-before">
                    <xsl:value-of select="$listAbove-4"/>
                </xsl:attribute>
                <xsl:attribute name="space-after">
                    <xsl:value-of select="$listBelow-4"/>
                </xsl:attribute>
            </xsl:when>
        </xsl:choose>
    </xsl:template>
    <xd:doc>
        <xd:short>Process elements tei:soCalled</xd:short>
        <xd:detail>&#160;</xd:detail>
    </xd:doc>
    <xsl:template match="tei:soCalled">
        <xsl:value-of select="$preQuote"/>
        <xsl:apply-templates/>
        <xsl:value-of select="$postQuote"/>
    </xsl:template>
    <xsl:template name="emphasize">
        <xsl:param name="class"/>
        <xsl:param name="content"/>
        <xsl:choose>
            <xsl:when test="$class='titlem'">
                <inline>
                    <xsl:attribute name="font-style">italic</xsl:attribute>
                    <xsl:copy-of select="$content"/>
                </inline>
            </xsl:when>
            <xsl:when test="$class='titlea'">
                <xsl:text>‘</xsl:text>
                <xsl:copy-of select="$content"/>
                <xsl:text>’</xsl:text>
            </xsl:when>
            <xsl:otherwise>
                <xsl:copy-of select="$content"/>
            </xsl:otherwise>
        </xsl:choose>
    </xsl:template>
    <xsl:template name="applyRendition"/>
</xsl:stylesheet>