<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns="http://www.w3.org/1999/XSL/Format" xmlns:tei="http://www.tei-c.org/ns/1.0" xmlns:fotex="http://www.tug.org/fotex" version="1.0">
    <xsl:import href="tei.xsl"/>
    
    
    
    <xsl:param name="tableAlign">left</xsl:param>
    
    <xsl:template match="tei:table">
        <xsl:choose>
            <xsl:when test="@rend='eqnarray' and $foEngine='passivetex'">
                <fotex:eqnarray>
                    <xsl:apply-templates select=".//tei:formula"/>
                </fotex:eqnarray>
            </xsl:when>
            <xsl:when test=".//tei:formula[@type='subeqn'] and $foEngine='passivetex'">
                <fotex:eqnarray>
                    <xsl:apply-templates select=".//tei:formula"/>
                </fotex:eqnarray>
            </xsl:when>
            <xsl:when test="$inlineTables or @rend='inline'">
                <xsl:if test="tei:head">
                    <block>
                        <xsl:call-template name="tableCaptionstyle"/>
                        
                        
                        
                        
                        <xsl:if test="$makeTableCaption='true'">
                            <xsl:call-template name="i18n">
                                <xsl:with-param name="word">tableWord</xsl:with-param>
                            </xsl:call-template>
                            <xsl:text> </xsl:text>
                            <xsl:call-template name="calculateTableNumber"/>
                            <xsl:text>. </xsl:text>
                        </xsl:if>
                        <xsl:apply-templates select="tei:head"/>
                    </block>
                </xsl:if>
                <xsl:call-template name="blockTable"/>
            </xsl:when>
            <xsl:otherwise>
                <xsl:call-template name="floatTable"/>
            </xsl:otherwise>
        </xsl:choose>
    </xsl:template>
</xsl:stylesheet>