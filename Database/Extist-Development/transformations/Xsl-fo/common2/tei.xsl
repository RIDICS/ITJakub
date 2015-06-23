<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:s="http://www.ascc.net/xml/schematron" xmlns:fo="http://www.w3.org/1999/XSL/Format" xmlns:tei="http://www.tei-c.org/ns/1.0" xmlns:sch="http://purl.oclc.org/dsdl/schematron" xmlns:xd="http://www.pnp-software.com/XSLTdoc" xmlns:fotex="http://www.tug.org/fotex" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" exclude-result-prefixes="xd s tei fotex xsi sch fo" version="2.0">
    <xsl:import href="tei-param.xsl"/>
    <xd:doc type="stylesheet">
        <xd:short> TEI stylesheet definitions common for all of HTML, FO and LaTeX
      outputs </xd:short>
        <xd:detail> This library is free software; you can redistribute it and/or
      modify it under the terms of the GNU Lesser General Public License as
      published by the Free Software Foundation; either version 2.1 of the
      License, or (at your option) any later version. This library is
      distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
      without even the implied warranty of MERCHANTABILITY or FITNESS FOR A
      PARTICULAR PURPOSE. See the GNU Lesser General Public License for more
      details. You should have received a copy of the GNU Lesser General Public
      License along with this library; if not, write to the Free Software
      Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA </xd:detail>
        <xd:author>See AUTHORS</xd:author>
        <xd:cvsId>$Id: tei.xsl 6640 2009-07-02 21:00:49Z rahtz $</xd:cvsId>
        <xd:copyright>2008, TEI Consortium</xd:copyright>
    </xd:doc>
    <xsl:include href="core.xsl"/>
    <xsl:include href="textstructure.xsl"/>
    <xsl:include href="header.xsl"/>
    <xsl:include href="linking.xsl"/>
    <xsl:include href="figures.xsl"/>
    <xsl:include href="i18n.xsl"/>
    <xd:doc type="string"> Uppercase letters.</xd:doc>
    <xsl:variable name="uc">ABCDEFGHIJKLMNOPQRSTUVWXYZ</xsl:variable>
    <xd:doc type="string"> Lowercase letters.</xd:doc>
    <xsl:variable name="lc">abcdefghijklmnopqrstuvwxyz</xsl:variable>
    <xd:doc type="string"> Name of XSLT processor.</xd:doc>
    <xsl:variable name="processor">
        <xsl:value-of select="system-property('xsl:vendor')"/>
    </xsl:variable>
    <xd:doc>
        <xd:short>[common] turn names into quote characters</xd:short>
        <xd:param name="quote">quote</xd:param>
        <xd:detail>&#160;</xd:detail>
    </xd:doc>
    <xsl:template name="getQuote">
        <xsl:param name="quote"/>
        <xsl:choose>
            <xsl:when test="$quote='laquo'">«</xsl:when>
            <xsl:when test="$quote='ldquo'">“</xsl:when>
            <xsl:when test="$quote='ldquor'">„</xsl:when>
            <xsl:when test="$quote='lsaquo'">‹</xsl:when>
            <xsl:when test="$quote='lsquo'">‘</xsl:when>
            <xsl:when test="$quote='lsquor'">‚</xsl:when>
            <xsl:when test="$quote='mdash'">—</xsl:when>
            <xsl:when test="$quote='raquo'">»</xsl:when>
            <xsl:when test="$quote='rdquo'">”</xsl:when>
            <xsl:when test="$quote='rdquor'">‟</xsl:when>
            <xsl:when test="$quote='rsaquo'">›</xsl:when>
            <xsl:when test="$quote='rsquo'">’</xsl:when>
            <xsl:when test="$quote='rsquor'">‛</xsl:when>
            <xsl:otherwise>?</xsl:otherwise>
        </xsl:choose>
    </xsl:template>
    <xsl:template name="whatsTheDate">
        <xsl:value-of select="format-dateTime(current-dateTime(),'[Y]-[M02]-[D02]T[H02]:[M02]:[s02]Z')"/>
    </xsl:template>
</xsl:stylesheet>