<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet exclude-result-prefixes="xd a fotex rng tei teix" version="2.0"
	xmlns="http://www.w3.org/1999/XSL/Format"
	xmlns:a="http://relaxng.org/ns/compatibility/annotations/1.0"
	xmlns:fotex="http://www.tug.org/fotex" xmlns:rng="http://relaxng.org/ns/structure/1.0"
	xmlns:tei="http://www.tei-c.org/ns/1.0" xmlns:teix="http://www.tei-c.org/ns/Examples"
	xmlns:xd="http://www.pnp-software.com/XSLTdoc" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:import href="ujc-ovj-prams.xsl"/>
	<xd:doc scope="stylesheet">
		<xd:desc>
			<xd:p><xd:b>Created on:</xd:b> Feb 17, 2011</xd:p>
			<xd:p><xd:b>Author:</xd:b> Boris</xd:p>
			<xd:p>Vlastní šablony a parametry pro generování PDF.</xd:p>
		</xd:desc>
	</xd:doc>


	<xsl:param name="documentationLanguage">cs</xsl:param>
	<xsl:param name="doclang">cs</xsl:param>

	<xd:doc class="output" type="string">
		<xd:short>Name of intended XSL FO engine</xd:short>
		<xd:detail>This is used to tailor the result for different XSL FO processors. By default, no
			special measures are taken, so there are no bookmarks or other such features. Possible
			values are <ul><li>passivetex (the TeX-based PassiveTeX processor</li><li>xep
					(XEP)</li><li>fop (FOP)</li><li>antenna (Antenna House)</li></ul>
		</xd:detail>
	</xd:doc>
	<!--<xsl:param name="foEngine"/>-->
	<xsl:param name="foEngine">fop</xsl:param>

	<xd:doc class="layout" type="string"> Which named page master name to use </xd:doc>
	<xsl:param name="forcePageMaster">simple1</xsl:param>


	<!--<xsl:variable name="vychoziPismo">Times New Roman</xsl:variable>-->
	<!--<xsl:variable name="vychoziPismo">Quivira</xsl:variable>-->
	<xsl:variable name="vychoziPismo">FreeSerif</xsl:variable>

	<xd:doc class="style" type="string"> Sans-serif font </xd:doc>
	<xsl:param name="sansFont">FreeSans</xsl:param>


	<xd:doc class="style" type="string">Default font for body</xd:doc>
	<xsl:param name="bodyFont">
		<xsl:value-of select="$vychoziPismo"/>
	</xsl:param>

	<xd:doc class="style" type="string">Font for section headings</xd:doc>
	<xsl:param name="divFont">
		<xsl:value-of select="$vychoziPismo"/>
	</xsl:param>


	<xd:doc class="style" type="string"> Font family for running header and footer </xd:doc>
	<!--	<xsl:param name="runFont">sans-serif</xsl:param>-->
	<xsl:param name="runFont">
		<xsl:value-of select="$sansFont"/>
	</xsl:param>

	<xd:doc class="style" type="string"> Font size for running header and footer </xd:doc>
	<xsl:param name="runSize">9pt</xsl:param>


	<xd:doc class="layout" type="boolean">Display section headings in running heads</xd:doc>
	<!--<xsl:param name="divRunningheads">true</xsl:param>-->
	<!-- TODO: nastavit živá záhlaví -->
	<xsl:param name="divRunningheads">true</xsl:param>

	<xd:doc class="style" type="string"> Default font size for body (without dimension) </xd:doc>
	<!--	<xsl:param name="bodyMaster">10</xsl:param>-->
	<xsl:param name="bodyMaster">12</xsl:param>

	<xd:doc>Velikost kramle u emendačních poznámek</xd:doc>
	<xsl:param name="kramleSize">9pt</xsl:param>

	<xd:doc class="style" type="string"> Calculation of normal body font size (add dimension) </xd:doc>
	<xsl:param name="bodySize">
		<xsl:value-of select="$bodyMaster"/>
		<xsl:text>pt</xsl:text>
	</xsl:param>

	<xd:doc class="style" type="string">Výchozí velikost písma v záhlví a zápatí stránky</xd:doc>
	<xsl:param name="headerSize">
		<xsl:choose>
			<xsl:when test="$rozmer = 'A6'">
				<xsl:value-of select="round($bodyMaster * 0.5)"/>
			</xsl:when>
			<xsl:otherwise>
				<xsl:value-of select="round($bodyMaster * 0.6)"/>
				<!--<xsl:attribute name="keep-with-next.within-page">always</xsl:attribute>-->
			</xsl:otherwise>
		</xsl:choose>
		<xsl:text>pt</xsl:text>
	</xsl:param>


	<xd:doc class="style" type="string"> Font size for display of title </xd:doc>
	<xsl:param name="titleSize">16pt</xsl:param>

	<xd:doc class="style" type="string">Font size for footnote numbers</xd:doc>
	<xsl:param name="footnotenumSize">6pt</xsl:param>

	<xd:doc class="style" type="string"> Font size for footnotes </xd:doc>
	<xsl:param name="footnoteSize">8pt</xsl:param>


	<xd:doc class="layout" type="boolean"> Make title page </xd:doc>
	<!--<xsl:param name="titlePage">true</xsl:param>-->
	<xsl:param name="titlePage">false</xsl:param>

	<xd:doc class="layout" type="string"> How to format page numbers in back matter (use XSLT number format) </xd:doc>
	<xsl:param name="formatBackpage">1</xsl:param>

	<xd:doc class="layout" type="string"> How to format page numbers in main matter (use XSLT number format) </xd:doc>
	<xsl:param name="formatBodypage">1</xsl:param>

	<xd:doc class="layout" type="string"> How to format page numbers in front matter (use XSLT number format) </xd:doc>
	<!--<xsl:param name="formatFrontpage">i</xsl:param>-->
	<!--<xsl:param name="formatFrontpage">I</xsl:param>-->
	<xsl:param name="formatFrontpage">I</xsl:param>

	<xd:doc class="style" type="boolean"> Hyphenate text </xd:doc>
	<!--<xsl:param name="hyphenate">true</xsl:param>-->
	<xsl:param name="hyphenate">false</xsl:param>

	<xd:doc class="output" type="string"> Language (for hyphenation) </xd:doc>
	<xsl:param name="language">cs</xsl:param>

	<xd:doc class="layout" type="boolean"> Make 2-page spreads </xd:doc>
	<!--<xsl:param name="twoSided">true</xsl:param>-->
	<xsl:param name="twoSided">false</xsl:param>


	<xd:doc class="layout" type="string"> Paragraph indentation. Odsazení odstavce a poznámky pod
		čarou.</xd:doc>
	<xsl:param name="parIndent">1em</xsl:param>

	<xd:doc class="layout" type="string"> Default spacing between paragraphs </xd:doc>
	<!--<xsl:param name="parSkip">0pt</xsl:param>-->
	<!--<xsl:param name="parSkip">0pt</xsl:param>-->

	<xsl:param name="parSkip">3pt</xsl:param>

	<xd:doc class="layout" type="string"> Maximum space allowed between paragraphs </xd:doc>
	<!--<xsl:param name="parSkipmax">12pt</xsl:param>-->
	<xsl:param name="parSkipmax">6pt</xsl:param>
	<!--<xsl:param name="parSkipmax">3pt</xsl:param>-->



	<xd:doc class="style" type="number">Výška řádku odstavce</xd:doc>
	<!--<xsl:param name="lineHeight">1.3</xsl:param>-->
	<xsl:param name="lineHeight">1.2</xsl:param>

	<xd:doc class="toc" type="boolean"> Make TOC for sections in &lt;back&gt; </xd:doc>
	<xsl:param name="tocBack">true</xsl:param>


	<xd:doc class="toc" type="boolean"> Make TOC for sections in &lt;front&gt; </xd:doc>
	<xsl:param name="tocFront">false</xsl:param>

	<!-- Popužívá se při generování obsahu -->
	<xd:doc class="style">
		<xd:short>[fo] Set attributes for display of links</xd:short>
		<xd:detail> </xd:detail>
	</xd:doc>
	<xsl:template name="linkStyle">
		<xsl:attribute name="text-decoration">none</xsl:attribute>
	</xsl:template>

	<xsl:template name="pageMasterHook">
		<simple-page-master master-name="obalka" page-width="{$pageWidth}"
			page-height="{$pageHeight}" margin-top="0pt" margin-bottom="0pt" margin-left="0pt"
			margin-right="0pt">
			<region-body column-count="{$columnCount}" margin-bottom="0pt" margin-top="0pt"/>
			<region-before region-name="xsl-region-before-first" extent="0pt"/>
			<region-after region-name="xsl-region-after-first" extent="0pt"/>
		</simple-page-master>
		<simple-page-master master-name="uvod1" page-width="{$pageWidth}"
			page-height="{$pageHeight}" margin-top="{$pageMarginTop}"
			margin-bottom="{$pageMarginBottom}" margin-left="{$pageMarginLeft}"
			margin-right="{$pageMarginRight}">
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


	<xd:doc>
		<xd:short>Process elements tei:front</xd:short>
		<xd:detail> </xd:detail>
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
				<xsl:if test="$titlePage='true'">
					<!-- no-force - není potřeba mít 2 titulní stránky -->
					<page-sequence format="{$formatFrontpage}" force-page-count="no-force"
						hyphenate="{$hyphenate}" language="{$language}">
						<xsl:call-template name="choosePageMaster">
							<xsl:with-param name="where">
								<xsl:value-of select="$frontMulticolumns"/>
							</xsl:with-param>
							<xsl:with-param name="force">
								<xsl:text>zacatek1</xsl:text>
							</xsl:with-param>
						</xsl:call-template>
						<static-content flow-name="xsl-region-before">
							<block/>
						</static-content>
						<static-content flow-name="xsl-region-after">
							<block/>
						</static-content>
						<flow flow-name="xsl-region-body" font-family="{$bodyFont}">
							<xsl:call-template name="Header"/>
						</flow>
					</page-sequence>
				</xsl:if>
				<!-- force-page-count="no-force"  -->
				<page-sequence format="{$formatFrontpage}" force-page-count="no-force"
					text-align="{$alignment}" hyphenate="{$hyphenate}" language="{$language}"
					initial-page-number="1">
					<xsl:call-template name="choosePageMaster">
						<xsl:with-param name="where">
							<xsl:value-of select="$frontMulticolumns"/>
						</xsl:with-param>
						<xsl:with-param name="force">
							<xsl:text>zacatek1</xsl:text>
						</xsl:with-param>
					</xsl:call-template>
					<!-- static areas -->
					<!-- fron je bez záhlaví a patičky -->
					<!--	<xsl:choose>
						<xsl:when test="$twoSided='true'">
							<xsl:call-template name="headers-footers-twoside"/>
						</xsl:when>
						<xsl:otherwise>
							<xsl:call-template name="headers-footers-oneside"/>
						</xsl:otherwise>
					</xsl:choose>-->
					<!-- now start the main flow -->
					<flow flow-name="xsl-region-body" font-family="{$bodyFont}"
						font-size="{$bodySize}">
						<xsl:for-each select="tei:*">
							<xsl:comment>Start <xsl:value-of select="name(.)"/></xsl:comment>
							<xsl:apply-templates select="."/>
							<xsl:comment>End <xsl:value-of select="name(.)"/></xsl:comment>
						</xsl:for-each>
					</flow>
				</page-sequence>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>

	<xd:doc>
		<xd:short>[fo] </xd:short>
		<xd:param name="where">where</xd:param>
		<xd:param name="force">force</xd:param>
		<xd:detail> </xd:detail>
	</xd:doc>
	<xsl:template name="choosePageMaster">
		<xsl:param name="where"/>
		<xsl:param name="force"/>
		<xsl:variable name="mn">
			<xsl:choose>
				<xsl:when test="not($force = '')">
					<xsl:value-of select="$force"/>
				</xsl:when>
				<xsl:when test="$forcePageMaster">
					<xsl:value-of select="$forcePageMaster"/>
				</xsl:when>
				<xsl:when test="not($where='')">
					<xsl:choose>
						<xsl:when test="$twoSided='true'">twoside2</xsl:when>
						<xsl:otherwise>oneside2</xsl:otherwise>
					</xsl:choose>
				</xsl:when>
				<xsl:otherwise>
					<xsl:choose>
						<xsl:when test="$twoSided='true'">twoside1</xsl:when>
						<xsl:otherwise>oneside1</xsl:otherwise>
					</xsl:choose>
				</xsl:otherwise>
			</xsl:choose>
		</xsl:variable>
		<xsl:attribute name="master-reference">
			<xsl:value-of select="$mn"/>
		</xsl:attribute>
	</xsl:template>

	<xd:doc class="style">
		<xd:short>[fo] Set attributes for display of heading for chapters (level 0)</xd:short>
		<xd:detail> </xd:detail>
	</xd:doc>
	<!-- Titul -->
	<xsl:template name="setupDiv0">
		<!--<xsl:attribute name="font-size">14pt</xsl:attribute>-->
		<xsl:attribute name="hyphenate">
			<xsl:text>false</xsl:text>
		</xsl:attribute>
		<xsl:attribute name="color">
			<xsl:value-of select="$barvaLoga"/>
		</xsl:attribute>
		<xsl:attribute name="font-size">16pt</xsl:attribute>
		<xsl:attribute name="text-align">left</xsl:attribute>
		<xsl:attribute name="font-weight">bold</xsl:attribute>
		<!--<xsl:attribute name="space-after.minimum">2pt</xsl:attribute>-->
		<xsl:attribute name="space-after.optimum">3pt</xsl:attribute>
		<xsl:attribute name="space-after.maximum">6pt</xsl:attribute>
		<!--		<xsl:attribute name="space-before.minimum">6pt</xsl:attribute>-->
		<xsl:attribute name="space-before.optimum">9pt</xsl:attribute>
		<xsl:attribute name="space-before.maximum">12pt</xsl:attribute>
		<xsl:attribute name="text-indent">
			<xsl:value-of select="$headingOutdent"/>
			<!-- zalomení stránky před oddílem -->
		</xsl:attribute>
		<xsl:attribute name="keep-with-next.within-page">always</xsl:attribute>
		<xsl:attribute name="page-break-before">always</xsl:attribute>
		<xsl:if test="@xml:id">
			<xsl:attribute name="id">
				<xsl:value-of select="@xml:id"/>
			</xsl:attribute>
		</xsl:if>
	</xsl:template>

	<xd:doc class="style">
		<xd:short>[fo] Set attributes for display of heading for 1st level sections</xd:short>
		<xd:detail> </xd:detail>
	</xd:doc>
	<xsl:template name="setupDiv1">
		<!--<xsl:attribute name="hyphenate"><xsl:text>false</xsl:text></xsl:attribute>-->
		<xsl:attribute name="color">
			<xsl:value-of select="$barvaLoga"/>
		</xsl:attribute>
		<xsl:attribute name="font-size">14pt</xsl:attribute>
		<xsl:attribute name="text-align">left</xsl:attribute>
		<!--<xsl:attribute name="font-weight">bold</xsl:attribute>-->
		<!--		<xsl:attribute name="space-after">6pt</xsl:attribute>
		<xsl:attribute name="space-before">9pt</xsl:attribute>
-->
		<!--<xsl:attribute name="space-after.minimum">2pt</xsl:attribute>-->
		<xsl:attribute name="space-after.optimum">5pt</xsl:attribute>
		<xsl:attribute name="space-after.maximum">7pt</xsl:attribute>
		<!--<xsl:attribute name="space-before.minimum">3pt</xsl:attribute>-->
		<xsl:attribute name="space-before.optimum">9pt</xsl:attribute>
		<xsl:attribute name="space-before.maximum">12pt</xsl:attribute>

		<xsl:attribute name="text-indent">
			<xsl:value-of select="$headingOutdent"/>
		</xsl:attribute>
		<xsl:choose>
			<xsl:when test="$rozmer = 'A6'">
				<xsl:attribute name="keep-with-next.within-page">always</xsl:attribute>
			</xsl:when>
			<xsl:otherwise>
				<xsl:attribute name="keep-with-next.within-page">auto</xsl:attribute>
				<!--<xsl:attribute name="keep-with-next.within-page">always</xsl:attribute>-->
			</xsl:otherwise>
		</xsl:choose>
		<xsl:if test="@xml:id">
			<xsl:attribute name="id">
				<xsl:value-of select="@xml:id"/>
			</xsl:attribute>
		</xsl:if>
	</xsl:template>
	<xd:doc class="style">
		<xd:short>[fo] Set attributes for display of heading for 2nd level sections </xd:short>
		<xd:detail> </xd:detail>
	</xd:doc>
	<xsl:template name="setupDiv2">
		<xsl:attribute name="color">
			<xsl:value-of select="$barvaLoga"/>
		</xsl:attribute>
		<xsl:attribute name="font-size">12pt</xsl:attribute>
		<xsl:attribute name="text-align">left</xsl:attribute>
		<xsl:attribute name="font-weight">normal</xsl:attribute>
		<!--<xsl:attribute name="font-style">italic</xsl:attribute>-->
		<!--		<xsl:attribute name="space-after">2pt</xsl:attribute>
<xsl:attribute name="space-before.optimum">4pt</xsl:attribute>-->
		<xsl:attribute name="space-after.maximum">4pt</xsl:attribute>
		<xsl:attribute name="space-after.optimum">2pt</xsl:attribute>
		<xsl:attribute name="space-before.maximum">8pt</xsl:attribute>
		<xsl:attribute name="space-before.optimum">6pt</xsl:attribute>

		<xsl:attribute name="text-indent">
			<xsl:value-of select="$headingOutdent"/>
		</xsl:attribute>
		<xsl:choose>
			<xsl:when test="$rozmer = 'A6'">
				<xsl:attribute name="keep-with-next.within-page">always</xsl:attribute>
			</xsl:when>
			<xsl:otherwise>
				<xsl:attribute name="keep-with-next.within-page">auto</xsl:attribute>
				<!--<xsl:attribute name="keep-with-next.within-page">always</xsl:attribute>-->
			</xsl:otherwise>
		</xsl:choose>
		<xsl:if test="@xml:id">
			<xsl:attribute name="id">
				<xsl:value-of select="@xml:id"/>
			</xsl:attribute>
		</xsl:if>
	</xsl:template>

	<xd:doc>
		<xd:short>Process elements tei:note</xd:short>
		<xd:detail> </xd:detail>
	</xd:doc>
	<xsl:template match="tei:note">
		<!-- Není třeba rozlišovat mezi odstavci na konci odstavce -->

		<!--<xsl:if test="name(preceding-sibling::*[1]) = 'add'">-->
		<xsl:if test="preceding-sibling::node()[1][self::add] and string-length(preceding-sibling::node()[1][self::text()]) = 0">
			<inline>
				<xsl:if test="not(@target)">
					<xsl:attribute name="font-size">
						<xsl:value-of select="$footnotenumSize"/>
					</xsl:attribute>
					<xsl:attribute name="vertical-align">super</xsl:attribute>
					<!-- netučná značka poznámky pod čarou -->
					<xsl:attribute name="font-style">normal</xsl:attribute>
					<xsl:attribute name="font-weight">normal</xsl:attribute>
					<xsl:text>,</xsl:text>
				</xsl:if>
			</inline>
		</xsl:if>

		<xsl:apply-templates mode="real" select="."/>

		<!--		<xsl:choose>
			<xsl:when test="ancestor::tei:p or ancestor::tei:item">
				<xsl:apply-templates select="." mode="real"/>
			</xsl:when>
			<xsl:otherwise>
				<block>
					<xsl:apply-templates select="." mode="real"/>
				</block>
			</xsl:otherwise>
		</xsl:choose>
-->
		<!-- pokud se sejdou dvě poznámky pod čarou, vloží se mezi ně čárka -->
		<xsl:call-template name="InsertCommaBeforeFollowingtNote" />
		
	</xsl:template>
	
	
	<xsl:template name="InsertCommaBeforeFollowingtNote">
		<xsl:if test="following-sibling::node()[1][self::tei:note]">
			<inline>
				<xsl:if test="not(@target)">
					<xsl:attribute name="font-size">
						<xsl:value-of select="$footnotenumSize" />
					</xsl:attribute>
					<xsl:attribute name="vertical-align">super</xsl:attribute>
					<!-- netučná značka poznámky pod čarou -->
					<xsl:attribute name="font-style">normal</xsl:attribute>
					<xsl:attribute name="font-weight">normal</xsl:attribute>
					<xsl:text>,</xsl:text>
				</xsl:if>
			</inline>
		</xsl:if>
	</xsl:template>
	
	<xd:doc>
		<xd:short>Process elements  tei:note properly</xd:short>
		<xd:detail> </xd:detail>
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
				<xsl:call-template name="makeFootnote"/>
				
<!--				<xsl:choose>
					<xsl:when test="parent::tei:item">
						<block>
							<xsl:call-template name="makeFootnote"/>
						</block>
					</xsl:when>
					<xsl:otherwise>
						<xsl:call-template name="makeFootnote"/>
					</xsl:otherwise>
				</xsl:choose>
-->
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>

	<xd:doc>
		<xd:short>Create a footnote</xd:short>
		<xd:detail> </xd:detail>
	</xd:doc>
	<!-- Úprava: font-weight="normal" -->
	<xsl:template name="makeFootnote">
		<xsl:param name="text"/>
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
		<footnote color="black">
			<inline>
				<xsl:if test="not(@target)">
					<xsl:attribute name="font-size">
						<xsl:value-of select="$footnotenumSize"/>
					</xsl:attribute>
					<xsl:attribute name="vertical-align">super</xsl:attribute>
					<!-- netučná značka poznámky pod čarou -->
					<xsl:attribute name="font-style">normal</xsl:attribute>
					<xsl:attribute name="font-weight">normal</xsl:attribute>
					<xsl:value-of select="$FootID"/>
				</xsl:if>
			</inline>
			<footnote-body>
				<block end-indent="0pt" font-size="{$footnoteSize}" font-style="normal"
					font-weight="normal" start-indent="0pt" text-align="start" text-indent="0pt">
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
					<!-- TODO: Zajistit odsazení celého textu poznámky -->
					<xsl:choose>
						<xsl:when test="$text">
							<xsl:value-of select="$text"/>
						</xsl:when>
						<xsl:otherwise>
							<xsl:apply-templates/>
						</xsl:otherwise>
					</xsl:choose>
				</block>
			</footnote-body>
		</footnote>
	</xsl:template>

	<xd:doc>
		<xd:short>[fo] process "rend" attribute</xd:short>
		<xd:param name="value">value of "rend" attribute</xd:param>
		<xd:detail> </xd:detail>
	</xd:doc>
	<xsl:template name="applyRend">
		<xsl:param name="value"/>

		<xsl:variable name="mezera">
			<xsl:choose>
				<xsl:when test="$rozmer = 'A6'">1em</xsl:when>
				<xsl:otherwise>2em</xsl:otherwise>
			</xsl:choose>
		</xsl:variable>
		<xsl:variable name="kvocient">
			<xsl:choose>
				<xsl:when test="$rozmer = 'A6'">1.3</xsl:when>
				<xsl:otherwise>2</xsl:otherwise>
			</xsl:choose>
		</xsl:variable>


		<xsl:choose>
			<xsl:when test="$value='gothic'">
				<xsl:attribute name="font-family">fantasy</xsl:attribute>
			</xsl:when>
			<xsl:when test="$value='calligraphic'">
				<xsl:attribute name="font-family">cursive</xsl:attribute>
			</xsl:when>
			<xsl:when
				test="$value='ital' or $value='italic' or $value='it' or $value='i' or $value='italics'">
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
				<xsl:attribute name="font-style">normal</xsl:attribute>
				<xsl:attribute name="font-weight">normal</xsl:attribute>
				<xsl:attribute name="font-size">
					<xsl:value-of select="$footnotenumSize"/>
				</xsl:attribute>
			</xsl:when>
			<!-- Doiplněno formátování pro superscript, např. transliteraci v horním indexu -->
			<xsl:when test="$value='superscript'">
				<xsl:attribute name="vertical-align">super</xsl:attribute>
				<xsl:attribute name="font-style">normal</xsl:attribute>
				<xsl:attribute name="font-weight">normal</xsl:attribute>
				<xsl:attribute name="font-size">
					<xsl:value-of select="$footnotenumSize"/>
				</xsl:attribute>
			</xsl:when>
			<xsl:when test="$value='normal'">
				<xsl:attribute name="font-weight">normal</xsl:attribute>
			</xsl:when>
			<xsl:when test="$value = 'right'">
				<xsl:attribute name="text-align">right</xsl:attribute>
			</xsl:when>
			<xsl:when test="$value = 'nazev'">
				<xsl:attribute name="text-indent">0em</xsl:attribute>
				<xsl:attribute name="font-size">
					<xsl:value-of select="$venovaniSize"/>
				</xsl:attribute>
			</xsl:when>
			<xsl:when test="$value = 'nazev2'">
				<xsl:attribute name="text-indent">0em</xsl:attribute>
				<xsl:attribute name="font-style">italic</xsl:attribute>
				<xsl:attribute name="font-size">
					<xsl:value-of select="$venovaniSize"/>
				</xsl:attribute>
			</xsl:when>
			<xsl:when test="$value = 'komu'">
				<xsl:attribute name="text-indent">0em</xsl:attribute>
				<xsl:attribute name="space-before">
					<xsl:value-of select="$mezera"/>
				</xsl:attribute>
				<xsl:attribute name="space-after">
					<xsl:value-of select="$mezera"/>
				</xsl:attribute>
				<xsl:attribute name="text-align">center</xsl:attribute>
				<xsl:attribute name="font-style">italic</xsl:attribute>
				<xsl:attribute name="font-size">
					<xsl:value-of select="concat(round($bodyMaster * $kvocient), 'pt')"/>
				</xsl:attribute>
				<xsl:attribute name="color">
					<xsl:value-of select="$barvaLoga"/>
				</xsl:attribute>
			</xsl:when>
			<xsl:when test="$value = 'vychazi'">
				<xsl:attribute name="text-indent">0em</xsl:attribute>
				<xsl:attribute name="space-before">
					<xsl:value-of select="$mezera"/>
				</xsl:attribute>
				<xsl:attribute name="space-after">
					<xsl:value-of select="$mezera"/>
				</xsl:attribute>
			</xsl:when>
		</xsl:choose>
	</xsl:template>


	<xsl:template name="makeItem">
		<!-- item behaviour depends on the type attribute of our parent:
simple, bullets, ordered, gloss, unordered, or bibliography
-->
		
		<xsl:choose>
			<xsl:when test="../@type='index' and name(../..) = 'cell'">
				<xsl:apply-templates />
			</xsl:when>
			<xsl:otherwise>
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
						<xsl:text>&#10;</xsl:text>
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
											<xsl:apply-templates mode="print"
												select="preceding-sibling::tei:*[1]"/>
										</xsl:otherwise>
									</xsl:choose>
								</xsl:when>
								<xsl:when test="../@type='numbered'">
									<!-- numbered support added rbl 26.3.2005 -->
									<xsl:attribute name="text-align">end</xsl:attribute>
									<xsl:number/>
									<xsl:text>.</xsl:text>
								</xsl:when>
								<xsl:when test="../@type='ordered'">
									<!-- numbered support added rbl 26.3.2005 -->
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
								<block font-weight="normal">
									<xsl:apply-templates/>
								</block>
								
								<!--<xsl:for-each select="*">
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
	    </xsl:for-each>-->
							</xsl:when>
							<xsl:otherwise>
								<block font-weight="normal">
									<xsl:apply-templates/>
								</block>
							</xsl:otherwise>
						</xsl:choose>
					</list-item-body>
				</list-item>
			</xsl:otherwise>
		</xsl:choose>
		
	</xsl:template>

	<xd:doc>
		<xd:short>[fo] </xd:short>
		<xd:detail> </xd:detail>
	</xd:doc>
	<xsl:template name="calculateFootnoteNumber">
		<xsl:choose>
			<xsl:when test="tei:choice">
				<xsl:number count="tei:note[tei:choice]" format="a" from="tei:text" level="any"/>
			</xsl:when>
			<!--			<xsl:when test="name(.) = 'add'">
				<xsl:number format="I"/>
			</xsl:when>
-->
			<xsl:otherwise>
				<xsl:number count="tei:note[not(tei:choice)] | tei:add" format="1" from="tei:text"
					level="any"/>
			</xsl:otherwise>
		</xsl:choose>


	</xsl:template>


	<xsl:template match="tei:figure[@type='cover' and @subtype='$rozmer' ]">
		<block-container height="100%" display-align="center">
			<block space-after="0pt" space-before="0pt" text-align="center">
				<external-graphic>
					<!--					<xsl:attribute name="width"><xsl:value-of select="$pageWidth"/> </xsl:attribute>
					<xsl:attribute name="height"><xsl:value-of select="$pageHeight"/></xsl:attribute>
-->
					<!--				<xsl:attribute name="content-width">scale-to-fit</xsl:attribute>
				<xsl:attribute name="content-height">scale-to-fit</xsl:attribute>
-->
					<!--				<xsl:attribute name="src">
					<xsl:value-of select="tei:graphic/@url" />
					</xsl:attribute>
					
					<xsl:call-template name="urlPodleRozmeru">
						<xsl:with-param name="url"><xsl:value-of select="tei:graphic/@url"/></xsl:with-param>
					</xsl:call-template>-->
					<xsl:attribute name="src">
						<xsl:text>url('</xsl:text>
						<xsl:value-of select="tei:graphic/@url"/>
						<xsl:text>')</xsl:text>
					</xsl:attribute>
				</external-graphic>
			</block>
		</block-container>
	</xsl:template>

	<xsl:template name="urlPodleRozmeru">
		<xsl:param name="url"/>
		<xsl:value-of select="substring-before($url, '.')"/>
		<xsl:text>_</xsl:text>
		<xsl:value-of select="$rozmer"/>
		<xsl:text>.</xsl:text>
		<xsl:value-of select="substring-after($url, '.')"/>
	</xsl:template>

	<xsl:template match="tei:figure">
		<xsl:choose>
			<xsl:when test="@subtype">
				<xsl:if test="@subtype = $rozmer">
					<block space-after="15pt" space-before="15pt" text-align="center">
						<xsl:if test="@type='cover'">
							<xsl:attribute name="page-break-after">
								<xsl:text>always</xsl:text>
							</xsl:attribute>
						</xsl:if>
						<external-graphic>
							<xsl:attribute name="src">
								<xsl:text>url('</xsl:text>
								<xsl:value-of select="tei:graphic/@url"/>
								<xsl:text>')</xsl:text>
							</xsl:attribute>
						</external-graphic>
					</block>
				</xsl:if>
			</xsl:when>
			<xsl:otherwise>
				<block space-after="15pt" space-before="15pt" text-align="center">
					<external-graphic>
						<xsl:attribute name="src">
							<xsl:text>url('</xsl:text>
							<xsl:value-of select="tei:graphic/@url"/>
							<xsl:text>')</xsl:text>
						</xsl:attribute>
					</external-graphic>
				</block>
			</xsl:otherwise>
		</xsl:choose>

	</xsl:template>

	<xsl:variable name="barvaLoga">
		<xsl:text>#383C98</xsl:text>
	</xsl:variable>

	<xsl:template match="tei:div[@type='venovani']">
		<block-container display-align="before" break-before="page">
			<block hyphenate="false" font-family="{$vychoziPismo}" font-size=" { $venovaniSize }"
				text-align="center" text-indent="0em">
				<xsl:apply-templates/>
			</block>
		</block-container>
	</xsl:template>


	<xsl:template match="tei:div[@type='vyroci']">
		<block color="{$barvaLoga}" font-family="{$vychoziPismo}" font-size="9pt"
			keep-with-next.within-page="always" space-after="12pt" space-before="0pt"
			text-align="center" text-indent="0em">
			<xsl:apply-templates/>
		</block>
	</xsl:template>

	<!--<xsl:template match="tei:body//tei:div[@type='editorial' and @subtype='comment']">
		<block font-family="{$vychoziPismo}" font-size="$bodySize"
			keep-with-next.within-page="always" space-after="12pt" space-before="9pt"
			text-align="justify" text-indent="0em">
			<xsl:apply-templates mode="section" select="tei:head"/>
			<xsl:apply-templates/>
		</block>
	</xsl:template>
-->
	<xsl:template match="tei:div[@type='editorial' and @sbtype='imprint']">
		<block font-family="{$vychoziPismo}" font-size="$bodySize"
			keep-with-next.within-page="always" space-after="12pt" space-before="9pt"
			text-align="left" text-indent="0em">

			<xsl:apply-templates/>
		</block>
	</xsl:template>

	<!--<xsl:template match="tei:front//tei:div[@type='editorial' and @subtype='annotation']">
		<block font-family="{$vychoziPismo}" font-size="12pt" font-style="italic"
			keep-with-next.within-page="always" space-after="12pt" space-before="9pt"
			text-align="justify" text-indent="0em">
			<xsl:apply-templates/>
		</block>
	</xsl:template>-->

	<xsl:template match="tei:figure" mode="loga">
		<external-graphic margin-left="5px">
			<xsl:attribute name="src">
				<xsl:text>url('</xsl:text>
				<xsl:value-of select="tei:graphic/@url"/>
				<xsl:text>')</xsl:text>
			</xsl:attribute>
		</external-graphic>
		<leader leader-pattern="space" leader-length.maximum="2em"/>
	</xsl:template>

	<xsl:template match="tei:div[@type='editorial' and @subtype='loga']">
		<block text-align="center" margin-top="20px" text-align-last="center">
			<xsl:choose>
				<xsl:when test="$rozmer = 'A4'">
					<xsl:apply-templates select="tei:figure[@subtype = 'A4']" mode="loga"/>
				</xsl:when>
				<xsl:otherwise>
					<xsl:apply-templates select="tei:figure[@subtype = 'A6']" mode="loga"/>
				</xsl:otherwise>
			</xsl:choose>
		</block>
	</xsl:template>

	<xsl:template match="tei:div[@type='editorial' and @subtype='edited']" priority="10">
		<block text-align="left" margin-top="20px" text-align-last="left">
			<xsl:apply-templates/>
		</block>
	</xsl:template>

	<xsl:template match="tei:table[@rend='podpisy']">
		<block space-before="2em">
			<table xmlns:m="http://www.w3.org/1998/Math/MathML" text-align="center"
				font-size="10.8pt" id="d0e130">
				<table-body text-indent="0pt" table-layout="fixed">
					<table-row>
						<table-cell padding="2pt" text-align="center" vertical-align="top">
							<!--									<block>ředitel Ústavu pro jazyk český AV ČR, v. v. i.</block>
<block>doc. RNDr. Karel Oliva, Dr.</block>-->
							<block>
								<xsl:value-of select="tei:row[1]/tei:cell[1]/tei:seg[2]"/>
							</block>
							<block>
								<xsl:value-of select="tei:row[1]/tei:cell[1]/tei:seg[3]"/>
							</block>
							<block overflow="visible">
								<xsl:apply-templates select="tei:row[1]/tei:cell[1]/tei:seg[1]"/>
							</block>
						</table-cell>
						<table-cell padding="2pt" text-align="center" vertical-align="top">
							<block>
								<xsl:value-of select="tei:row[1]/tei:cell[2]/tei:seg[2]"/>
							</block>
							<block>
								<xsl:value-of select="tei:row[1]/tei:cell[2]/tei:seg[3]"/>
							</block>
							<block overflow="visible">
								<xsl:apply-templates select="tei:row[1]/tei:cell[2]/tei:seg[1]"/>
							</block>
						</table-cell>
					</table-row>
				</table-body>
			</table>

		</block>
	</xsl:template>

	<!--<xsl:template match="tei:table[@rend='podpisy']//tei:cell">
		
	</xsl:template>-->

	<xsl:template match="tei:cell/tei:seg">
		<xsl:apply-templates/>
	</xsl:template>

	<xd:doc>
		<xd:short>Process element tei:p</xd:short>
		<xd:detail> </xd:detail>
	</xd:doc>
	<xsl:template match="tei:p">
		<block line-height="{$lineHeight}">
			<xsl:if test="preceding-sibling::tei:p">
				<!-- neodsazení druhého atd. řádku jenom v rámci editorialu -->
				<!--<xsl:if test="not(ancestor::tei:div[@type='editorial' and @subtype='imprint'])">-->
				<xsl:if test="not(ancestor::tei:div[@type='editorial'])">
					<xsl:attribute name="text-indent">
						<xsl:value-of select="$parIndent"/>
					</xsl:attribute>
				</xsl:if>
				<xsl:attribute name="space-before.optimum">
					<xsl:value-of select="$parSkip"/>
				</xsl:attribute>
				<xsl:attribute name="space-before.maximum">
					<xsl:value-of select="$parSkipmax"/>
				</xsl:attribute>
			</xsl:if>
			<xsl:if test="ancestor::tei:div[@type='editorial' and @subtype='imprint']">
				<xsl:attribute name="space-after">
					<xsl:value-of select="'2em'"/>
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
			<!-- Odsadit celý tok textu -->
			<block>
				<xsl:if test="@rend">
					<xsl:call-template name="applyRend">
						<xsl:with-param name="value">
							<xsl:value-of select="@rend"/>
						</xsl:with-param>
					</xsl:call-template>
				</xsl:if>
				<xsl:apply-templates/>
			</block>
		</block>
	</xsl:template>

	<xd:doc>
		<xd:short>Process elements tei:list</xd:short>
		<xd:detail> </xd:detail>
	</xd:doc>
	<xsl:template match="tei:list|tei:listBibl">
		<xsl:if test="child::tei:head">
			<block font-style="normal" text-align="start" space-before.optimum="4pt"
				keep-with-next="always">
				<xsl:for-each select="tei:head">
					<xsl:apply-templates/>
				</xsl:for-each>
			</block>
		</xsl:if>
		<xsl:choose>
			<xsl:when test="@type='index' and parent::tei:cell">
				<xsl:apply-templates />
			</xsl:when>
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
		<xd:short>Process elements tei:pb</xd:short>
		<xd:detail> </xd:detail>
	</xd:doc>
	<xsl:template match="tei:pb">
		<xsl:variable name="e">
			<xsl:choose>
				<xsl:when
					test="parent::tei:body or parent::tei:front or
					parent::tei:back or parent::tei:div">
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
				<xsl:element name="{$e}" xmlns="http://www.w3.org/1999/XSL/Format">
					<xsl:attribute name="break-before">page</xsl:attribute>
					<xsl:if test="@xml:id">
						<xsl:attribute name="id">
							<xsl:value-of select="@xml:id"/>
						</xsl:attribute>
					</xsl:if>
				</xsl:element>
			</xsl:when>
			<xsl:when test="$pagebreakStyle='visible'">
				<xsl:element name="{$e}" xmlns="http://www.w3.org/1999/XSL/Format">
					<xsl:if test="@xml:id">
						<xsl:attribute name="id">
							<xsl:value-of select="@xml:id"/>
						</xsl:attribute>
					</xsl:if>
					<!-- nastavení vlastností hranice strany pro potřeby EE -->
					<xsl:attribute name="font-weight">bold</xsl:attribute>
					<xsl:attribute name="font-size">
						<xsl:value-of select="$bodySize"/>
					</xsl:attribute>
					<xsl:attribute name="color">#000000</xsl:attribute>
					<!--					<xsl:attribute name="border">10pt</xsl:attribute>
-->
					<!--<xsl:attribute name="margin">10pt</xsl:attribute>-->
					<!--<xsl:attribute name="padding">10pt</xsl:attribute>-->
					<!--					<xsl:attribute name="padding-start">2pt</xsl:attribute>
					<xsl:attribute name="padding-end">2pt</xsl:attribute>
-->
					
					<!-- držení hranatých závorek pohormadě -->
					
					<xsl:attribute name="keep-together.within-line">
						<xsl:value-of select="'always'"/>
					</xsl:attribute>
					<xsl:text>[</xsl:text>
					<xsl:value-of select="@n"/>
					<xsl:text>]</xsl:text>
					<xsl:if test="@rend = 'space'">
						<xsl:text> </xsl:text>
					</xsl:if>

					<!--<xsl:choose>
						<xsl:when test="@rend = 'space'">
							<xsl:text>[</xsl:text>
							<xsl:value-of select="@n"/>
							<xsl:text>] </xsl:text>
						</xsl:when>
						<xsl:otherwise>
							<character character="[" keep-with-next="always"
								keep-with-previous="always"/>
							<xsl:value-of select="@n"/>
							<character character="]" keep-with-next="always"
								keep-with-previous="always"/>
						</xsl:otherwise>
					</xsl:choose>-->
				</xsl:element>
			</xsl:when>
		</xsl:choose>
	</xsl:template>

	<xd:doc>
		<xd:short>Process elements tei:corr</xd:short>
		<xd:detail> </xd:detail>
	</xd:doc>
	<xsl:template match="tei:corr">
		<inline font-style="italic">
			<xsl:apply-templates/>
		</inline>
		<!-- větší velikost závorky -->
		<inline font-size="{$kramleSize}">
			<xsl:text>] </xsl:text>
		</inline>
		<xsl:choose>
			<xsl:when test="@sic">
				<footnote>
					<footnote-citation>
						<inline font-size="7pt" vertical-align="super">
							<xsl:number count="tei:corr" format="a" level="any"/>
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
										<xsl:number count="tei:corr" format="a" level="any"/>
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
		<xd:short>Process elements tei:sic</xd:short>
		<xd:detail> </xd:detail>
	</xd:doc>
	<xsl:template match="tei:sic">
		<xsl:apply-templates/>
		<!--<xsl:text> (sic)</xsl:text>-->
	</xsl:template>

	<xd:doc>
		<xd:short>Process elements tei:seg</xd:short>
		<xd:detail> </xd:detail>
	</xd:doc>
	<xsl:template match="tei:seg[@type='bible' and @subtype='verse']">
		<xsl:text> </xsl:text>
		<inline>
			<xsl:attribute name="font-size">
				<xsl:value-of select="$footnotenumSize"/>
			</xsl:attribute>
			<xsl:attribute name="vertical-align">super</xsl:attribute>
			<!-- netučná značka poznámky pod čarou -->
			<xsl:attribute name="font-style">normal</xsl:attribute>
			<xsl:attribute name="font-weight">normal</xsl:attribute>
			<xsl:value-of select="translate(substring-after(@xml:id, '.'), '.', ',')"/>
			<xsl:if test="name(child::*[1]) = 'note' and not(text())">
				<xsl:text>, </xsl:text>
			</xsl:if>
		</inline>
		<xsl:apply-templates/>
	</xsl:template>

	<xd:doc>
		<xd:short>[fo] </xd:short>
		<xd:param name="runhead">runhead</xd:param>
		<xd:detail> </xd:detail>
	</xd:doc>
	<xsl:template name="headers-footers-oneside">
		<xsl:param name="runhead"/>
		<static-content flow-name="xsl-region-before">
			<block color="gray" height="{$regionBeforeExtent}" font-family="{$runFont}"
				font-size="{$headerSize}" text-align="center" overflow="hidden">
				<inline>
					<!--	<leader leader-pattern="space" leader-length.minimum="3em" />-->
					<!--<retrieve-marker retrieve-class-name="section0"/>
					<xsl:text> – </xsl:text>-->
					<!-- first-starting-within-page -->
					<!-- last-starting-within-page -->
					<!--					<xsl:text>(fs) </xsl:text>
					<retrieve-marker retrieve-class-name="section1" retrieve-position="first-starting-within-page"/>
					<xsl:text>; (fc) </xsl:text>
					<retrieve-marker retrieve-class-name="section1" retrieve-position="first-including-carryover"/>
					<xsl:text>; (le) </xsl:text>
					<retrieve-marker retrieve-class-name="section1" retrieve-position="last-starting-within-page"/>
					<xsl:text>; (le) </xsl:text>
					<retrieve-marker retrieve-class-name="section1" retrieve-position="last-ending-within-page"/>
-->
				</inline>
			</block>
		</static-content>
		<static-content flow-name="xsl-region-after">
			<block color="gray" font-family="{$runFont}" font-size="{$headerSize}"
				text-align-last="justify">
				<xsl:choose>
					<xsl:when test="$runhead='true'">
						<xsl:value-of select="$runhead"/>
					</xsl:when>
					<xsl:otherwise>
						<!-- Bez jména autora v záhlaví -->
						<!--<xsl:call-template name="runninghead-author"/>-->
						<!-- uvést aspoň titul díla -->
						<inline>

							<!--							<xsl:text>(fs) </xsl:text>
							<retrieve-marker retrieve-class-name="section1" retrieve-position="first-starting-within-page"/>
							<xsl:text>; (fc) </xsl:text>
							<retrieve-marker retrieve-class-name="section1" retrieve-position="first-including-carryover"/>
							<xsl:text>; (le) </xsl:text>
							<retrieve-marker retrieve-class-name="section1" retrieve-position="last-starting-within-page"/>
							<xsl:text>; (le) </xsl:text>
							<retrieve-marker retrieve-class-name="section1" retrieve-position="last-ending-within-page"/>
-->
							<xsl:text>– </xsl:text>
							<page-number/>
							<xsl:text> –</xsl:text>
						</inline>
						<xsl:if test="$divRunningheads='true'">
							<leader leader-pattern="space"/>
							<xsl:call-template name="runninghead-title"/>
						</xsl:if>

					</xsl:otherwise>
				</xsl:choose>
			</block>
		</static-content>
		<static-content flow-name="xsl-region-before-first">
			<!-- Chyba - chybějící obsah prvku static -->
			<block/>
		</static-content>
		<static-content flow-name="xsl-region-after-first">
			<block color="gray" font-family="{$runFont}" font-size="{$headerSize}"
				text-align="center">
				<!--<xsl:text>– </xsl:text>
				<page-number />
				<xsl:text> –</xsl:text>-->
				<!--<xsl:call-template name="runninghead-title" />-->
			</block>
		</static-content>
		<static-content flow-name="xsl-footnote-separator">
			<block>
				<leader color="{$barvaLoga}" leader-length="20%" leader-pattern="rule"
					rule-style="solid" rule-thickness="0.5pt"/>
			</block>
		</static-content>
	</xsl:template>

	<xd:doc>
		<xd:short>Process elements tei:gap</xd:short>
		<xd:detail> </xd:detail>
	</xd:doc>
	<xsl:template match="tei:gap">
		<!--		<inline border-style="solid">
			<xsl:text>[</xsl:text>
			<xsl:value-of select="@reason"/>
			<xsl:text>]</xsl:text>
		</inline>
-->
		<xsl:text>…</xsl:text>
	</xsl:template>

	<xsl:template match="tei:add">
		<xsl:text>{</xsl:text>
		<xsl:apply-templates/>
		<xsl:text>}</xsl:text>
		<xsl:call-template name="makeFootnote">
			<xsl:with-param name="text">
				<xsl:call-template name="pripisek"/>
				<!--<xsl:if test="name(following-sibling::*[1]) = 'note'">
					<xsl:text>; </xsl:text>
					<xsl:apply-templates select="following-sibling::*[1]/*" />
				</xsl:if>-->
			</xsl:with-param>
		</xsl:call-template>
		<xsl:call-template name="InsertCommaBeforeFollowingtNote" />
	</xsl:template>

	<xsl:template name="pripisek">
		<xsl:choose>
			<xsl:when test="@place = 'margin'">marginální</xsl:when>
			<xsl:when test="@place = 'inline'">interlineární</xsl:when>
		</xsl:choose>
		<xsl:text> přípisek </xsl:text>
		<xsl:choose>
			<xsl:when test="@type = 'contemporaneous'">soudobou rukou</xsl:when>
			<xsl:when test="@type = 'non-contemporaneous'">pozdější rukou</xsl:when>
		</xsl:choose>
	</xsl:template>

	<xsl:template match="tei:supplied">
		<inline>
			<xsl:attribute name="keep-together.within-line">
				<xsl:value-of select="'always'"/>
			</xsl:attribute>
		<xsl:text>[</xsl:text>
		<xsl:apply-templates/>
		<xsl:text>]</xsl:text>
		</inline>
	</xsl:template>
	<xsl:template match="tei:titlePage">
		<!--	<xsl:if test="$rozmer = 'A4'">
	<block>
		<xsl:apply-templates select="tei:figure[@type='cover']" />
	</block>
	</xsl:if>
-->
		<xsl:apply-templates select="tei:figure[@type='cover']"/>

		<block color="{$barvaLoga}" hyphenate="false" font-size=" { $venovaniSize }">
			<xsl:apply-templates select="tei:docTitle"/>
			<xsl:apply-templates select="tei:docAuthor"/>
		</block>
		<xsl:apply-templates select="tei:div[@type='editorial' and @subtype='annotation']"/>
	</xsl:template>

	<xd:doc>
		<xd:short>Process elements tei:l</xd:short>
		<xd:detail> </xd:detail>
	</xd:doc>
	<xsl:template match="tei:l[@n]">
		<block space-after.optimum="0pt" space-before.optimum="0pt" text-align-last="start">
			<xsl:choose>
				<xsl:when test="starts-with(@rend,'indent(')">
					<xsl:attribute name="text-indent">
						<xsl:value-of
							select="concat(substring-before(substring-after(@rend,'('),')'),'em')"/>
					</xsl:attribute>
				</xsl:when>
				<xsl:when test="starts-with(@rend,'indent')">
					<!--<xsl:attribute name="text-indent">1em</xsl:attribute>-->
				</xsl:when>
			</xsl:choose>

			<!--<xsl:attribute name="start-indent">1em</xsl:attribute>-->
			<!--<xsl:attribute name="text-indent">2em</xsl:attribute>-->
			<xsl:apply-templates select="@n"/>
			<xsl:apply-templates/>
		</block>
	</xsl:template>

	<xsl:template match="tei:l/@n">
		<inline>
			<xsl:attribute name="font-size">
				<xsl:value-of select="$footnotenumSize"/>
			</xsl:attribute>
			<xsl:attribute name="vertical-align">middle</xsl:attribute>
			<!--<xsl:attribute name="left">-1em</xsl:attribute>-->
			<xsl:attribute name="position">relative</xsl:attribute>
			<!-- netučná značka poznámky pod čarou -->
			<xsl:attribute name="font-style">normal</xsl:attribute>
			<xsl:attribute name="font-weight">normal</xsl:attribute>
			<xsl:choose>
				<xsl:when test=". = 1 or . mod 5 = 0">
					<xsl:text>(</xsl:text>
					<xsl:value-of select="."/>
					<xsl:text>)</xsl:text>
				</xsl:when>
				<xsl:otherwise>
					<xsl:attribute name="color">white</xsl:attribute>
					<xsl:text>(</xsl:text>
					<xsl:value-of select="."/>
					<xsl:text>)</xsl:text>
				</xsl:otherwise>
			</xsl:choose>
		</inline>
		<leader leader-length.minimum="2em" leader-pattern="space"/>
	</xsl:template>


	<xd:doc>
		<xd:short>[fo] </xd:short>
		<xd:detail> </xd:detail>
	</xd:doc>
	<xsl:template name="mainTOC">
		<block page-break-before="always">
			<xsl:call-template name="setupDiv0"/>
			<xsl:call-template name="i18n">
				<xsl:with-param name="word">contentsWord</xsl:with-param>
			</xsl:call-template>
		</block>
		<xsl:choose>
			<xsl:when test="ancestor::tei:text/tei:group">
				<xsl:for-each select="ancestor::tei:text/tei:group">
					<xsl:apply-templates mode="toc" select="tei:text"/>
				</xsl:for-each>
			</xsl:when>
			<xsl:when test="ancestor::tei:text/tei:body/tei:div1">
				<xsl:if test="$tocFront='true'">
					<xsl:for-each
						select="ancestor::tei:text/tei:front/tei:div1|ancestor::tei:text/tei:front//tei:div2|ancestor::tei:text/tei:front//tei:div3">
						<xsl:apply-templates mode="toc" select="(.)"/>
					</xsl:for-each>
				</xsl:if>
				<xsl:for-each
					select="ancestor::tei:text/tei:body/tei:div1|ancestor::tei:text/tei:body//tei:div2|ancestor::tei:text/tei:body//tei:div3">
					<xsl:apply-templates mode="toc" select="(.)"/>
				</xsl:for-each>
				<xsl:if test="$tocBack='true'">
					<xsl:for-each
						select="ancestor::tei:text/tei:back/tei:div1|ancestor::tei:text/tei:back//tei:div2|ancestor::tei:text/tei:back//tei:div3">
						<xsl:apply-templates mode="toc" select="(.)"/>
					</xsl:for-each>
				</xsl:if>
			</xsl:when>
			<xsl:otherwise>
				<xsl:call-template name="tocBits"/>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>

	<xsl:template match="tei:body/tei:div//tei:div |  tei:back/tei:div//tei:div" mode="toc"/>

	<!-- pouze první úroveň nadpisů -->
	<xsl:template match="tei:body/tei:div | tei:back/tei:div " mode="toc">
		<!--<xsl:template match="tei:body/tei:div | tei:body/tei:div/tei:div | tei:back/tei:div | tei:back/tei:div/tei:div" mode="toc">-->
		<!-- Položka obsahu se zpracuje pouze tehdy, když má nadpis -->
		<xsl:if test="tei:head">
			<block text-align="left" text-align-last="justify" hyphenate="false" line-height="1.5em"
				start-indent="2em" end-indent="1em" text-indent="-1em">
				<inline>
					<basic-link internal-destination="{@xml:id}">
						<!--<xsl:value-of select="tei:head"/>-->
<!--						<xsl:call-template name="makeRunningHeadText"/>-->
						<xsl:apply-templates select="tei:head" mode="toc" />
					</basic-link>
					<leader leader-pattern="dots"/>
					<basic-link internal-destination="{@xml:id}">
						<page-number-citation ref-id="{@xml:id}"/>
					</basic-link>
				</inline>
			</block>
			<!--<xsl:apply-templates select="section" mode="toc"/>-->
		</xsl:if>
	</xsl:template>

	<xsl:template match="tei:head" mode="toc">
		<xsl:apply-templates mode="toc" />
	</xsl:template>

	<xsl:template match="tei:pb | tei:note " mode="toc" />


	<xsl:template match="tei:hi" mode="toc">
		<xsl:apply-templates select="." />
	</xsl:template>



	<xsl:template match="section" mode="toc">
		<block start-indent="1em" text-align-last="justify" text-indent="-1em">
			<inline padding-start="1em">
				<xsl:value-of select="title"/>
				<leader leader-pattern="dots"/>
				<page-number-citation ref-id="{@xml:id}"/>
			</inline>
		</block>
	</xsl:template>

	<xd:doc>
		<xd:short>[fo] </xd:short>
		<xd:detail> </xd:detail>
	</xd:doc>
	<xsl:template name="tocBits">
		<xsl:if test="$tocFront='true'">
			<xsl:for-each select="ancestor::tei:text/tei:front//tei:div">
				<xsl:apply-templates mode="toc" select="(.)"/>
			</xsl:for-each>
		</xsl:if>
		<!--<xsl:for-each select="ancestor::tei:text/tei:body/tei:div">-->
		<xsl:for-each
			select="ancestor::tei:text/tei:body/tei:div|ancestor::tei:text/tei:body/tei:div/tei:div">
			<xsl:apply-templates mode="toc" select="(.)"/>
		</xsl:for-each>
		<xsl:if test="$tocBack='true'">
			<xsl:for-each select="ancestor::tei:text/tei:back//tei:div">
			<!--<xsl:for-each select="ancestor::tei:text/tei:back/tei:div">--><!-- pouze 1 úroveň nadpisů -->
				<xsl:apply-templates mode="toc" select="(.)"/>
			</xsl:for-each>
		</xsl:if>
	</xsl:template>
	<xd:doc>
		<xd:short>Process elements tei:div (toc mode)</xd:short>
		<xd:detail>
			<p> headings in TOC </p>
		</xd:detail>
	</xd:doc>
	<!--	<xsl:template match="tei:div" mode="toc">
		<xsl:variable name="divlevel">
			<xsl:value-of select="count(ancestor::tei:div)"/>
		</xsl:variable>
		<xsl:if test="$divlevel &lt; 1">
		<xsl:call-template name="tocheading">
			<xsl:with-param name="level">
				<xsl:value-of select="$divlevel"/>
			</xsl:with-param>
		</xsl:call-template>
		</xsl:if>
		</xsl:template>-->

	<xsl:template match="tei:div[tei:p[1][not (node())]]">
		<xsl:text>&#10;</xsl:text>
		<!--<block line-height="{$lineHeight}" />-->
	</xsl:template>

	<xsl:template match="tei:head" mode="section">
		<!-- if we have multiple <head> elements together, 
  separate by spaces -->
		<!--
   <xsl:if test="preceding-sibling::tei:head">
	<xsl:text> </xsl:text>
     </xsl:if>
-->
		<block keep-with-next="always">
			<xsl:apply-templates/>
		</block>
	</xsl:template>

	<xd:doc>
		<xd:short>Process elements tei:div</xd:short>
		<xd:detail>
			<p> Normal headings </p>
		</xd:detail>
	</xd:doc>
	<xsl:template match="tei:div">
		<xsl:text>&#10;</xsl:text>
		<xsl:choose>
			<xsl:when test="@type='bibliog'">
				<xsl:apply-templates/>
			</xsl:when>
			<xsl:when test="@type='abstract'">
				<block keep-with-next.within-page="always" end-indent="{$exampleMargin}"
					start-indent="{$exampleMargin}">
					<xsl:attribute name="text-align">center</xsl:attribute>
					<xsl:call-template name="setupDiv2"/>
					<inline font-style="italic">Abstract</inline>
				</block>
				<xsl:apply-templates/>
			</xsl:when>
			<xsl:when test="@type='ack'">
				<block keep-with-next.within-page="always">
					<xsl:attribute name="text-align">start</xsl:attribute>
					<xsl:call-template name="setupDiv3"/>
					<inline font-style="italic">Acknowledgements</inline>
				</block>
				<xsl:apply-templates/>
			</xsl:when>
			<!-- anotace, která je součástí titulky, se neodsazuje, netvoří oddíl -->
			<xsl:when test="@type='editorial' and @subtype='annotation'">
				<xsl:apply-templates/>
			</xsl:when>
			<xsl:otherwise>
				<!-- behaviour depends on the nesting level of <div> elements -->
				<xsl:variable name="divlevel" select="count(ancestor::tei:div)"/>
				<xsl:call-template name="NumberedHeading">
					<xsl:with-param name="level">
						<xsl:value-of select="$divlevel"/>
					</xsl:with-param>
				</xsl:call-template>
				<xsl:apply-templates/>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>

	<xd:doc>
		<xd:short>[fo] </xd:short>
		<xd:param name="level">level</xd:param>
		<xd:detail> </xd:detail>
	</xd:doc>
	<xsl:template name="NumberedHeading">
		<xsl:param name="level"/>
		<block>
			<xsl:choose>
				<xsl:when test="$rozmer = 'A6'">
					<xsl:attribute name="keep-with-next.within-page">always</xsl:attribute>
				</xsl:when>
				<xsl:otherwise>
					<xsl:attribute name="keep-with-next.within-page">auto</xsl:attribute>
					<!--<xsl:attribute name="keep-with-next.within-page">always</xsl:attribute>-->
				</xsl:otherwise>
			</xsl:choose>
			<!--<block keep-with-next.within-page="always">-->
			<!--<block keep-with-next.within-page="auto">-->
			<xsl:variable name="divid">
				<xsl:choose>
					<xsl:when test="@xml:id">
						<xsl:value-of select="@xml:id"/>
					</xsl:when>
					<xsl:otherwise>
						<xsl:value-of select="generate-id()"/>
					</xsl:otherwise>
				</xsl:choose>
			</xsl:variable>
			<xsl:attribute name="id">
				<xsl:value-of select="$divid"/>
			</xsl:attribute>
			<xsl:attribute name="text-align">start</xsl:attribute>
			<xsl:attribute name="font-family">
				<xsl:value-of select="$divFont"/>
			</xsl:attribute>
			<xsl:choose>
				<xsl:when test="$level=0">
					<xsl:call-template name="setupDiv0"/>
				</xsl:when>
				<xsl:when test="$level=1">
					<xsl:call-template name="setupDiv1"/>
				</xsl:when>
				<xsl:when test="$level=2">
					<xsl:call-template name="setupDiv2"/>
				</xsl:when>
				<xsl:when test="$level=3">
					<xsl:call-template name="setupDiv3"/>
				</xsl:when>
				<xsl:when test="$level=4">
					<xsl:call-template name="setupDiv4"/>
				</xsl:when>
				<xsl:when test="$level=5">
					<xsl:call-template name="setupDiv5"/>
				</xsl:when>
			</xsl:choose>
			<xsl:call-template name="blockStartHook"/>
			<xsl:variable name="Number">
				<xsl:if test="$numberHeadings='true' and $numberHeadingsDepth &gt; $level">
					<xsl:call-template name="calculateNumber">
						<xsl:with-param name="numbersuffix">
							<xsl:call-template name="headingNumberSuffix"/>
						</xsl:with-param>
					</xsl:call-template>
				</xsl:if>
			</xsl:variable>
			<!--
				<xsl:message>**  Calculated   [<xsl:value-of select="$Number"/>] [<xsl:value-of select="$headingNumberSuffix"/>] for <xsl:value-of select="@xml:id"/></xsl:message>
			-->
			<xsl:value-of select="$Number"/>
			<xsl:if test="$divRunningheads='true'">
				<!-- markers for use in running heads -->
				<xsl:choose>
					<xsl:when test="$level=0">
						<marker marker-class-name="section1"/>
						<marker marker-class-name="section2"/>
						<marker marker-class-name="section3"/>
						<marker marker-class-name="section4"/>
						<marker marker-class-name="section5"/>
					</xsl:when>
					<xsl:when test="$level=1">
						<marker marker-class-name="section2"/>
						<marker marker-class-name="section3"/>
						<marker marker-class-name="section4"/>
						<marker marker-class-name="section5"/>
					</xsl:when>
					<xsl:when test="$level=2">
						<marker marker-class-name="section3"/>
						<marker marker-class-name="section4"/>
						<marker marker-class-name="section5"/>
					</xsl:when>
					<xsl:when test="$level=3">
						<marker marker-class-name="section4"/>
						<marker marker-class-name="section5"/>
					</xsl:when>
					<xsl:when test="$level=4">
						<marker marker-class-name="section5"/>
					</xsl:when>
					<xsl:when test="$level=5"/>
				</xsl:choose>
				<marker marker-class-name="section{$level}">
					<xsl:if test="$numberHeadings='true'">
						<xsl:value-of select="$Number"/>
						<xsl:call-template name="headingNumberSuffix"/>
					</xsl:if>
					<!--<xsl:value-of select="tei:head"/>-->
					<xsl:call-template name="makeRunningHeadText"/>

				</marker>
			</xsl:if>
			<xsl:apply-templates mode="section" select="tei:head"/>
			<xsl:choose>
				<xsl:when test="$foEngine='passivetex'">
					<!-- Passive TeX extension, to get PDF bookmarks -->
					<fotex:bookmark fotex-bookmark-level="{$level}" fotex-bookmark-label="{$divid}">
						<xsl:if test="$numberHeadings='true'">
							<xsl:value-of select="$Number"/>
						</xsl:if>
						<xsl:value-of select="tei:head"/>
					</fotex:bookmark>
				</xsl:when>
			</xsl:choose>
		</block>
	</xsl:template>
	<xsl:template name="makeRunningHeadText">
		<xsl:variable name="textNadpisu">
			<xsl:apply-templates select="tei:head" mode="runningHead"/>
		</xsl:variable>
		<!-- Dodělat - lépe vymyslet krácení dlouhého nadpisu... -->
		<xsl:choose>
			<xsl:when test="string-length(normalize-space($textNadpisu)) &gt; 150">
				<xsl:choose>
					<xsl:when test="(string-length(substring-before($textNadpisu, '.')) &lt; 150) and string-length(substring-before($textNadpisu, '.')) &gt; 0">
						<xsl:value-of select="concat(substring-before($textNadpisu, '.'), '…')"/>
					</xsl:when>
					<xsl:when test="(string-length(substring-before($textNadpisu, ',')) &lt; 150) and string-length(substring-before($textNadpisu, ',')) &gt; 0">
						<xsl:value-of select="concat(substring-before($textNadpisu, ','), '…')"/>
					</xsl:when>
					<xsl:otherwise>
						<xsl:value-of select="concat(substring($textNadpisu, 1, 150), '…')"/>
					</xsl:otherwise>
				</xsl:choose>
				
			</xsl:when>
			<xsl:otherwise>
				<xsl:value-of select="normalize-space($textNadpisu)"/>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>

	<xsl:template match="tei:head" mode="runningHead">
		<xsl:apply-templates mode="runningHead"/>
	</xsl:template>

	<xsl:template match="tei:pb | tei:note" mode="runningHead"/>

	<xsl:template match="*" mode="runningHead">
		<xsl:apply-templates/>
	</xsl:template>

</xsl:stylesheet>
