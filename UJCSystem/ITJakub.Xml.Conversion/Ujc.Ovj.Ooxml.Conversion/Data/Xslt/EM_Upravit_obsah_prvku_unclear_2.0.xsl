<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:xd="http://www.oxygenxml.com/ns/doc/xsl" xmlns:xml="http://www.w3.org/XML/1998/namespace" xmlns:vwf="http://vokabular.ujc.cas.cz/xslt/functions" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:fn="http://www.w3.org/2005/xpath-functions" extension-element-prefixes="vwf fn" exclude-result-prefixes="xd vwf xs" version="2.0">
	<xsl:import href="Kopirovani_prvku.xsl" />
	<xsl:import href="Textove_funkce.xsl" />
	<!--<xsl:import href="EM_Upravit_obsah_prvku_unclear_2.0_elementy.xsl"/>-->

	<xd:doc scope="stylesheet">
		<xd:desc>
			<xd:p><xd:b>Created on:</xd:b> Dec 3, 2010</xd:p>
			<xd:p><xd:b>Author:</xd:b> boris</xd:p>
			<xd:p />
		</xd:desc>
	</xd:doc>


	<xsl:output omit-xml-declaration="no" indent="no" />
	<xsl:strip-space elements="unclear supplied" />

	<xsl:variable name="interpunkce" select="'?.,;!:„“‚‘’#'" />
	<xsl:variable name="mezera" select="' '" />
	<xsl:variable name="interpunkcePlusMezera" select="concat($interpunkce, $mezera)" />

	<xsl:template match="/">
		<xsl:comment> EM_Upravit_obsah_prvku_unclear_2.0 </xsl:comment>
		<xsl:apply-templates />
	</xsl:template>

	<xd:doc id="unclear-after-element">
		<xd:desc />
	</xd:doc>
	<xd:doc>
		<xd:desc>
			<xd:p></xd:p>
		</xd:desc>
	</xd:doc>
	<xsl:template match="unclear[preceding-sibling::node()[not(self::text()[normalize-space()=''])][1][self::*]]" priority="3">

		<xsl:call-template name="process-unclear">
			<xsl:with-param name="current-text">
				<xsl:copy-of select="supplied/text()" />
			</xsl:with-param>

			<xsl:with-param name="source-previous-text">
				<xsl:value-of select="preceding-sibling::*/text()[last()]" />
			</xsl:with-param>

	<!-- XXX5 -->
			<xsl:with-param name="source-next-text">
				
				<xsl:choose>
					<xsl:when test="following-sibling::node()[1]/self::text()">
						<xsl:value-of select="following-sibling::node()[1]" />
					</xsl:when>
					<xsl:otherwise>
						<xsl:value-of select="following-sibling::*/text()[1]" />
					</xsl:otherwise>
				</xsl:choose>
				
			</xsl:with-param>

			<xsl:with-param name="previous-node">
				<xsl:value-of select="preceding-sibling::node()[1]" />
			</xsl:with-param>

			<xsl:with-param name="next-node">
				<xsl:value-of select="following-sibling::node()[1]" />
			</xsl:with-param>
			
			<xsl:with-param name="source-next-next-node">
				<xsl:if test="following-sibling::node()[2]/self::unclear">
					<xsl:value-of  select="following-sibling::node()[2]" />
				</xsl:if>
			</xsl:with-param>

		</xsl:call-template>



	</xsl:template>

	<xd:doc id="unclear-before-element">
		<xd:desc />
	</xd:doc>
	<xsl:template match="unclear[following-sibling::node()[not(self::text()[normalize-space()=''])][1][self::*]]" priority="3">

		<xsl:call-template name="process-unclear">
			<xsl:with-param name="current-text">
				<xsl:copy-of select="supplied/text()" />
			</xsl:with-param>

			<xsl:with-param name="source-previous-text">
				<xsl:value-of select="preceding-sibling::*/text()[last()]" />
			</xsl:with-param>

			<xsl:with-param name="source-next-text">
				<xsl:value-of select="following-sibling::*/text()[1]" />
			</xsl:with-param>
		</xsl:call-template>

	</xsl:template>


	<xsl:template match="unclear[following-sibling::node()[not(self::text()[normalize-space()=''])]]">

		<xsl:call-template name="process-unclear">
			
			<xsl:with-param name="current-text">
				<xsl:copy-of select="supplied/text()" />
			</xsl:with-param>

			<xsl:with-param name="source-previous-text">
				<xsl:value-of select="preceding-sibling::*/text()[last()]" />
			</xsl:with-param>

			<xsl:with-param name="source-next-text">
				<xsl:value-of select="parent::*[1]/text()[not(self::text()[normalize-space()=''])][1]" />
			</xsl:with-param>
			
			<xsl:with-param name="previous-node">
				<xsl:value-of select="preceding-sibling::*[1]" />
			</xsl:with-param>
			
			<xsl:with-param name="next-node">
				<xsl:value-of select="following-sibling::node()[1]" />
			</xsl:with-param>
			
			<xsl:with-param name="source-next-next-node">
				<xsl:value-of select="following-sibling::node()[2]
					[not(self::text()[normalize-space()=''])[1]]" />
			</xsl:with-param>
			
		</xsl:call-template>
	

	</xsl:template>

	<xd:doc id="unclear-before-unclear-and-after-supplied">
		<xd:desc />
	</xd:doc>
	<xsl:template match="unclear   [preceding-sibling::*[1]/self::supplied]   [following-sibling::*[1][self::text][not(contains(text()[1], ' '))]   and following-sibling::*[1]/child::*[1]/self::unclear   ]" priority="20">


		<xsl:call-template name="process-unclear">
			<xsl:with-param name="current-text">
				<xsl:copy-of select="supplied/text()" />
			</xsl:with-param>

			<xsl:with-param name="source-previous-text">
				<xsl:value-of select="preceding-sibling::*[1]/text()[last()]" />
			</xsl:with-param>

			<xsl:with-param name="source-next-text">
				<xsl:value-of select="following-sibling::*[1]/text()[1]" />
			</xsl:with-param>

			<xsl:with-param name="previous-node">
				<xsl:value-of select="preceding-sibling::*[1]" />
			</xsl:with-param>

			<xsl:with-param name="next-node">
				<xsl:choose>
					<xsl:when test="following-sibling::node()[1]
						[not(self::text()[normalize-space()=''])[1]]/self::text()">
						<xsl:value-of select="following-sibling::node()[1]
							[not(self::text()[normalize-space()=''])[1]]" />
					</xsl:when>
					<xsl:otherwise>
						<xsl:value-of select="following-sibling::node()[1]
							[not(self::text()[normalize-space()=''])[1]]/node()[1]" />
					</xsl:otherwise>
				</xsl:choose>
			</xsl:with-param>

		<xsl:with-param name="source-next-next-node">
			 <xsl:choose>
			 	<xsl:when test="following-sibling::node()[1]/self::text">
			 		<xsl:value-of select="following-sibling::node()[1]/node()[2]
			 			[not(self::text()[normalize-space()=''])[1]]" />
			 	</xsl:when>
			 	<xsl:when test="following-sibling::node()[2]/node()[1]/self::text()">
			 		<xsl:value-of select="following-sibling::node()[2]/node()[2]"/>
			 	</xsl:when>
			 	<xsl:otherwise>
			 		<xsl:value-of select="following-sibling::node()[2]
			 			" />
			 	</xsl:otherwise>
			 <!--	<xsl:otherwise>
			 		<xsl:value-of select="following-sibling::node()[2]
			 			[not(self::text()[normalize-space()=''])[1]]" />
			 	</xsl:otherwise>-->
			 </xsl:choose>
		</xsl:with-param>

		</xsl:call-template>


	</xsl:template>

	<xd:doc id="unclear-before-and-after-element">
		<xd:desc />
	</xd:doc>
	<xsl:template match="unclear[preceding-sibling::node()[not(self::text()[normalize-space()=''])][1][self::*]    and following-sibling::node()[not(self::text()[normalize-space()=''])][1][self::*]]" priority="10">

		<xsl:call-template name="process-unclear">
			<xsl:with-param name="current-text">
				<xsl:copy-of select="supplied/text()" />
			</xsl:with-param>

			<xsl:with-param name="source-previous-text">
				<xsl:value-of select="preceding-sibling::*[node()][1]/text()[last()]" />
			</xsl:with-param>

			<xsl:with-param name="source-next-text">
				<xsl:value-of select="following-sibling::*[1]/text()[1]" />
			</xsl:with-param>
			
			<xsl:with-param name="previous-node">
				<xsl:value-of select="preceding-sibling::node()[1]" />
			</xsl:with-param>

			<xsl:with-param name="next-node">
				<xsl:choose>
					<xsl:when test="following-sibling::node()[1]/self::*[*]">
						<xsl:value-of select="following-sibling::node()[1]/node()[1]"/>
					</xsl:when>
					<xsl:otherwise>
						<xsl:value-of select="following-sibling::node()[not(self::text()[normalize-space()=''])][1]" />		
						
<!--						<xsl:value-of select="following-sibling::node()[not(normalize-space()='')][1]" />		
-->					</xsl:otherwise>
				</xsl:choose>
			</xsl:with-param>
			
			<xsl:with-param name="source-next-next-node">
				<xsl:value-of select="following-sibling::node()[2]
					[not(self::text()[normalize-space()=''])[1]]" />
			</xsl:with-param>
			
		</xsl:call-template>


	</xsl:template>


	<xsl:template match="pb[not(@rend='space')][preceding-sibling::*[1]/self::unclear] | pb[not(@rend='space')][following-sibling::*[1]/self::unclear] | 
		pb[not(@rend='space')][following-sibling::*[1][not(contains(node()[1], $mezera))][node()[2]/self::unclear]] |
		cb[not(@rend='space')][preceding-sibling::*[1]/self::unclear] | cb[not(@rend='space')][following-sibling::*[1]/self::unclear] | 
		cb[not(@rend='space')][following-sibling::*[1][not(contains(node()[1], $mezera))][node()[2]/self::unclear]] |
		lb[not(@rend='space')][preceding-sibling::*[1]/self::unclear] | lb[not(@rend='space')][following-sibling::*[1]/self::unclear] | 
		lb[not(@rend='space')][following-sibling::*[1][not(contains(node()[1], $mezera))][node()[2]/self::unclear]]" priority="5" />
	
	<xsl:template match="pb[@rend='space'][following-sibling::*[1][self::unclear][not(contains(., $mezera))]]" priority="5" />

	<xsl:template match="text[preceding-sibling::*[1]/(self::pb | self::cb)][preceding-sibling::*[2]/self::unclear][contains(text()[1], ' ')]" priority="20">
		<xsl:choose>
			<xsl:when test="contains(normalize-space(), ' ')">
				<xsl:if test="contains(normalize-space(text()[1]), ' ')">
				<text>
					<xsl:call-template name="vlozit-text-za-interpunkci-vcetne">
						<xsl:with-param name="text" select="text()[1]" />
					</xsl:call-template>
				</text>
				</xsl:if>
				<xsl:if test="node()[position() > 1]">
					<text><xsl:apply-templates select="node()[position() > 1]" /></text>
				</xsl:if>
			</xsl:when>
			<xsl:otherwise>
				<xsl:if test="node()[position() > 1]">
					<text><xsl:apply-templates select="node()[position() > 1]" /></text>
				</xsl:if>
			</xsl:otherwise>
		</xsl:choose>
		
		
	</xsl:template>
	<!-- XXX3 -->
	<xsl:template match="
		text[not(unclear)][following-sibling::*[1]/(self::pb | self::cb)][following-sibling::*[2]/self::unclear] | 
		supplied[not(unclear)][following-sibling::*[1]/(self::pb | self::cb)][following-sibling::*[2]/self::unclear] |
		text[not(unclear)][following-sibling::*[1]/(self::pb | self::cb)][following-sibling::*[2]/self::text[not(contains( node()[1], $mezera))][node()[2]/self::unclear]]
		" priority="20">
		<xsl:variable name="element" select="name()"/>
		<xsl:for-each select="node()">
			<xsl:choose>
				<xsl:when test="position() &lt; last()">
					<xsl:choose>
						<xsl:when test="self::text()">
							<xsl:element name="{$element}">
								<xsl:value-of select="current()"/>
							</xsl:element>
						</xsl:when>
						<xsl:otherwise>
							<xsl:apply-templates select="current()" />
						</xsl:otherwise>
					</xsl:choose>
				</xsl:when>
			</xsl:choose>
			<xsl:if test="position() = last()">
				<xsl:element name="{$element}">
					<xsl:value-of select="vwf:substring-before-last-from-many-including(current(), $interpunkcePlusMezera)" />
				</xsl:element>
			</xsl:if>
		</xsl:for-each>
	</xsl:template>
	
	<xd:doc id="unclear-before-empty-element">
		<xd:desc />
	</xd:doc>
	<xsl:template match="unclear[preceding-sibling::node()[not(self::text()[normalize-space()=''])][1][self::*]    and following-sibling::node()[not(self::text()[normalize-space()=''])][1][self::*] and following-sibling::*[1]/(self::pb | self::cb)]" priority="12">
		
		<xsl:call-template name="process-unclear">
			<xsl:with-param name="current-text">
				<xsl:copy-of select="supplied/text()" />
			</xsl:with-param>
			
			<xsl:with-param name="source-previous-text">
				<xsl:value-of select="preceding-sibling::*[1]/text()[last()]" />
			</xsl:with-param>
			
			<xsl:with-param name="source-next-text">
				<xsl:value-of select="following-sibling::*[node()][1]/text()[1]" />
			</xsl:with-param>
			
			<xsl:with-param name="previous-node">
				<xsl:value-of select="preceding-sibling::node()[1]" />
			</xsl:with-param>
			
			<xsl:with-param name="next-node">
				<xsl:value-of select="following-sibling::*[1][not(node())][1]"/>
			</xsl:with-param>
			
			<xsl:with-param name="source-next-next-node">
				<xsl:value-of select="following-sibling::node()[not(self::text()[normalize-space()=''])][2]" />
			</xsl:with-param>
			
		</xsl:call-template>
		
		
	</xsl:template>

	<xd:doc id="unclear-preceded-by-text">
		<xd:desc>
			<xd:p>Případy, kdy je rekonstrukce/unclear v rámci popisku k obrázku, tj. je obklopen přímo textem, nikoli elementy.</xd:p>
			<xd:p>
				<xd:a href="http://stackoverflow.com/questions/21598317/xslt-check-if-a-particular-element-is-preceeded-by-a-text-node-why-this-does-no" />
			</xd:p>
			<xd:p>
				<xd:a href="http://stackoverflow.com/questions/2613159/xslt-and-xpath-match-preceding-comments" />
			</xd:p>
		</xd:desc>
	</xd:doc>
	<xsl:template match="unclear[preceding-sibling::node()[1][not(self::text()[normalize-space()=''])[1]]]" priority="5">
		<xsl:call-template name="process-unclear">

			<xsl:with-param name="current-text">
				<xsl:choose>
					<!--<xsl:when test="contains(supplied, $mezera)">
						<xsl:value-of select="supplied/text()" />
					</xsl:when>-->
					<!--<xsl:when test=" not(contains(normalize-space(supplied), $mezera)) 
						and vwf:ends-with(supplied, $mezera)" >
						<xsl:value-of select="$mezera" />
					</xsl:when>-->
				<!--	<xsl:when test="parent::*[1]/preceding-sibling::node()[1]/self::unclear 
						and not(contains(parent::*[1]/preceding-sibling::*[1]/self::unclear, $mezera))
						and contains(normalize-space(supplied), $mezera)
						">
						<xsl:value-of select="concat($mezera, substring-after(supplied/text(), $mezera))" />
					</xsl:when>-->
					<xsl:when test="
						preceding-sibling::node()[1][self::text()]/not(contains(., $mezera)) and
						preceding-sibling::node()[2][self::unclear]/not(contains(., $mezera)) and
						preceding-sibling::node()[3][self::text()]/(contains(., $mezera)) and
						ends-with(., $mezera) and
						not(contains(normalize-space(.), $mezera))
						">
						<xsl:value-of select="''" />
					</xsl:when>
					<xsl:when test="contains(normalize-space(supplied), $mezera)">
						<xsl:value-of select="supplied/text()" />
					</xsl:when>
					<xsl:when test="preceding-sibling::node()[1]/self::text() and contains(preceding-sibling::node()[1], $mezera)">
						<xsl:copy-of select="supplied/text()" />
					</xsl:when>
					<xsl:when test="parent::*[1]/preceding-sibling::node()[1]/self::unclear and not(contains(parent::*[1]/preceding-sibling::*[1]/self::unclear, $mezera))">
						<xsl:value-of select="''" />
					</xsl:when>
					<xsl:otherwise>
						<xsl:copy-of select="supplied/text()" />
					</xsl:otherwise>
				</xsl:choose>
			</xsl:with-param>

			<xsl:with-param name="source-previous-previous-text">
				<xsl:if test="
					not (contains(preceding-sibling::node()[1], $mezera)) and
					parent::*[1]/preceding-sibling::*[1]/self::*[not(node())][not(@rend='space')]
					and contains(parent::*[1]/preceding-sibling::*[2]/text()[last()], $mezera)
					">
					<xsl:value-of select="vwf:substring-after-last-from-many(parent::*[1]/preceding-sibling::*[2]/text()[last()], $interpunkcePlusMezera)"/>
				</xsl:if>
			</xsl:with-param>

			<xsl:with-param name="source-previous-text">
				<xsl:choose>
					
					<xsl:when test="preceding-sibling::node()[2]/self::unclear 
						and not(contains(preceding-sibling::node()[2], $mezera)) 
						and not(contains(preceding-sibling::node()[1], $mezera))">
						<xsl:value-of select="''" />
					</xsl:when>
					
					<xsl:when test="parent::*[1]/preceding-sibling::node()[1]/self::unclear 
						and not(contains(preceding-sibling::node()[1], $mezera)) 
						and not(contains(parent::*[1]/preceding-sibling::node()[1], $mezera))">
						<xsl:value-of select="''" />
					</xsl:when>

					<xsl:when test="preceding-sibling::node()[1]/self::text() != '' and not(contains(normalize-space(preceding-sibling::node()[1]), $mezera))">
<!--						<xsl:value-of select="''" />-->
						<!-- XXX2 -->
						<xsl:value-of select="preceding-sibling::node()[1]" />
					</xsl:when>
					
					<!-- Lépe definovat podmínku; musí bezprostředně následovat -->
					<xsl:when test="preceding-sibling::node()[1]/self::text()">
						<xsl:value-of select="preceding-sibling::node()[1]" />
					</xsl:when>
					<xsl:when test="parent::*[1]/preceding-sibling::node()[1]/self::unclear 
						and not(contains(parent::*[1]/preceding-sibling::*[1]/self::unclear, $mezera))">
						<xsl:value-of select="''" />
					</xsl:when>
					

					
					<xsl:otherwise>
						<xsl:value-of select="''" />
					</xsl:otherwise>
				</xsl:choose>
			</xsl:with-param>

			<xsl:with-param name="source-next-text">
				<xsl:choose>
					<xsl:when test="following-sibling::node()[1]/self::text()">
						<xsl:value-of select="following-sibling::node()[1]" />
					</xsl:when>
					<xsl:otherwise>
						<xsl:value-of select="''" />
					</xsl:otherwise>
				</xsl:choose>
			</xsl:with-param>

			<xsl:with-param name="next-node">
				<xsl:if test="following-sibling::node()[1]/self::unclear">
					<xsl:value-of select="following-sibling::node()[1]" />
				</xsl:if>
			</xsl:with-param>

			<xsl:with-param name="previous-node">
				<xsl:if test="preceding-sibling::node()[1]/self::unclear">
					<xsl:value-of select="preceding-sibling::node()[1]" />
				</xsl:if>
			</xsl:with-param>
			
			<xsl:with-param name="source-next-next-node">
				<xsl:if test="following-sibling::node()[2]/self::unclear">
					<xsl:value-of  select="following-sibling::node()[2]" />
				</xsl:if>
			</xsl:with-param>

		</xsl:call-template>
	</xsl:template>

	<xsl:template match="unclear
		[not(contains(., ' '))]
		[
		preceding-sibling::node()[1]
		[not(self::text()[normalize-space()=''])[1]]
		]
		[not(contains(
		preceding-sibling::node()[1]
		[not(self::text()[normalize-space()=''])[1]],
		' ')
		)
		]
		[
		preceding-sibling::node()[2]
		[not(self::text()[normalize-space()=''])[1]]
		]
		[not(contains(
		preceding-sibling::node()[2]
		[not(self::text()[normalize-space()=''])[1]],
		' ')
		)
		]" priority="30" >
		<xsl:message> 30 </xsl:message>
	</xsl:template>
		

	<xsl:template match="unclear
		[not(contains(., ' '))]
		[
		following-sibling::node()[1]
		[not(self::text()[normalize-space()=''])[1]]
		]
		[not(contains(
		following-sibling::node()[1]
		[not(self::text()[normalize-space()=''])[1]],
		' ')
		)
		]
		[
		following-sibling::node()[2]
		[not(self::text()[normalize-space()=''])[1]]
		]
		[not(contains(
		following-sibling::node()[2]
		[not(self::text()[normalize-space()=''])[1]],
		' ')
		)
		]" priority="30">
		
			<xsl:call-template name="process-unclear">
				
				<xsl:with-param name="current-text">
					<xsl:value-of select="supplied/text()" />
				</xsl:with-param>
				
				<xsl:with-param name="source-previous-text">
					<xsl:value-of select="preceding-sibling::node()[1]
						[not(self::text()[normalize-space()=''])[1]]" />
				</xsl:with-param>
				
				<xsl:with-param name="next-node">
					<xsl:value-of select="following-sibling::node()[1]
						[not(self::text()[normalize-space()=''])[1]]" />
				</xsl:with-param>
				
				<xsl:with-param name="previous-node">
					<xsl:value-of select="preceding-sibling::node()[1]
						[not(self::text()[normalize-space()=''])[1]]" />
				</xsl:with-param>
				
				
				<xsl:with-param name="source-next-next-node">
					<xsl:value-of select="following-sibling::node()[2]
						[not(self::text()[normalize-space()=''])[1]]" />
				</xsl:with-param>

				<xsl:with-param name="source-next-text">
					<xsl:choose>
						<xsl:when test="following-sibling::node()[1]
							[not(self::text()[normalize-space()=''])[1]]/self::text()">
							<xsl:value-of select="following-sibling::node()[1]
								[not(self::text()[normalize-space()=''])[1]]" />
						</xsl:when>
						<xsl:otherwise>
							<xsl:value-of select="following-sibling::node()[1]
								[not(self::text()[normalize-space()=''])[1]]/text()" />
						</xsl:otherwise>
					</xsl:choose>
				</xsl:with-param>
				
				
				
			</xsl:call-template>
		
	</xsl:template>
	
	<xsl:template match="text[preceding-sibling::*[1]/self::unclear][not(contains(text()[1], ' '))][child::*[1]/self::unclear] " priority="20">
		<xsl:apply-templates select="node()[position() > 1]" />
	</xsl:template>

	<xsl:template match="text[preceding-sibling::*[1]/self::unclear][not(contains(text()[1], ' '))][child::*[1][self::unclear][contains(., $mezera)]] " priority="25">
		<xsl:apply-templates select="node()[position() > 2]" />
	</xsl:template>

	<xd:doc id="text-before-unclear-and-after-element">
		<xd:desc />
	</xd:doc>
	<xsl:template match="text()[1][not(self::text()[normalize-space()=''])[1]][following-sibling::node()[1]/self::unclear]   [parent::*[1]/preceding-sibling::*[1][self::unclear]]" priority="5">

		<xsl:call-template name="process-text-node">
			<xsl:with-param name="current-text" select="." />
			<xsl:with-param name="next-unclear">
				<xsl:copy-of select="following-sibling::node()[1]/self::unclear/supplied/text()" />
			</xsl:with-param>
			<xsl:with-param name="previous-unclear">
				<xsl:value-of select="parent::*[1]/preceding-sibling::*[1]/self::unclear/supplied/text()" />
			</xsl:with-param>
		</xsl:call-template>

	</xsl:template>


	<xd:doc id="text-before-unclear">
		<xd:desc>
			<xd:p>Případy, kdy je prvek rekonstrukce/unclear součástí obklopujícího textu; vybere text, který prvku rekonstrukce/unclear předchází.</xd:p>
			<xd:p>
				<xd:a href="http://stackoverflow.com/questions/21598317/xslt-check-if-a-particular-element-is-preceeded-by-a-text-node-why-this-does-no" />
			</xd:p>
			<xd:p>
				<xd:a href="http://stackoverflow.com/questions/2613159/xslt-and-xpath-match-preceding-comments" />
			</xd:p>
		</xd:desc>
	</xd:doc>
	<xsl:template match="text()[not(self::text()[normalize-space()=''])[1]][following-sibling::node()[1]/self::unclear]">

		<xsl:call-template name="process-text-node">
			<xsl:with-param name="current-text" select="." />
			<xsl:with-param name="next-unclear">
				<xsl:copy-of select="following-sibling::node()[1]/self::unclear/supplied/text()" />
			</xsl:with-param>
			<xsl:with-param name="previous-unclear">
				<xsl:value-of select="preceding-sibling::node()[1]/self::unclear/supplied/text()" />
			</xsl:with-param>
		</xsl:call-template>

	</xsl:template>
<!-- XXX5 -->
	<xsl:template match="text()[not(self::text()[normalize-space()=''])[1]][preceding-sibling::node()[1]/self::unclear[contains(normalize-space(.), $mezera)]][contains(normalize-space(preceding-sibling::node()[1]), $mezera)]
		[contains(preceding::node()[not(normalize-space() = '')][self::text()][2], $mezera)]" priority="12">
		
		<xsl:call-template name="process-text-node">
			<xsl:with-param name="current-text">
				<xsl:choose>
					<xsl:when test="
						contains(.,$mezera) and
						preceding-sibling::node()[1]/self::unclear and
						not(contains(normalize-space(preceding-sibling::node()[1]/supplied/text()), $mezera)) and
						not(ends-with(normalize-space(preceding-sibling::node()[1]/supplied/text()), $mezera)) and
						following-sibling::node()[1]/self::unclear and
						not(ends-with(normalize-space(following-sibling::node()[1]/supplied/text()), $mezera))">
						<xsl:value-of select="vwf:substring-before-last-from-many-including(., $mezera)"/>
					</xsl:when>
					<xsl:when test="preceding-sibling::node()[1]/self::unclear and
						not(contains(normalize-space(preceding-sibling::node()[1]/supplied/text()), $mezera)) and
						not(ends-with(normalize-space(preceding-sibling::node()[1]/supplied/text()), $mezera))">
						<xsl:value-of select="."/>
					</xsl:when>
					<xsl:when test="following-sibling::node()[1]/self::unclear and
						not(ends-with(., $mezera))">
						<xsl:variable name="znak">
							<xsl:call-template name="najitPrvniVyskytujiciSeZnak">
								<xsl:with-param name="text" select="." />
								<xsl:with-param name="interpunkce" select="$interpunkcePlusMezera" />
							</xsl:call-template>
						</xsl:variable>
						<xsl:variable name="temp" select="vwf:substring-after-first-from-many(., $interpunkcePlusMezera)" />
						<xsl:value-of select="concat($znak, vwf:substring-before-last($temp, $mezera), $mezera)"/>
					</xsl:when>
					<xsl:otherwise>
						<xsl:variable name="znak">
							<xsl:call-template name="najitPrvniVyskytujiciSeZnak">
								<xsl:with-param name="text" select="." />
								<xsl:with-param name="interpunkce" select="$interpunkcePlusMezera" />
							</xsl:call-template>
						</xsl:variable>
						
						<xsl:choose>
							<xsl:when test="$znak = $mezera">
									<xsl:value-of select="vwf:substring-after-first-from-many(., $interpunkcePlusMezera)"/>									</xsl:when>
							<xsl:otherwise>
								<xsl:value-of select="concat($znak, vwf:substring-after-first-from-many(., $interpunkcePlusMezera))"/>	
							</xsl:otherwise>
						</xsl:choose>
					</xsl:otherwise>
				</xsl:choose>
				
			</xsl:with-param>
			<xsl:with-param name="next-unclear">
				<xsl:value-of select="''" />
			</xsl:with-param>
			<xsl:with-param name="previous-unclear">
				<xsl:value-of select="''" />
			</xsl:with-param>
		</xsl:call-template>
		
		
	</xsl:template>
	
	
	<xsl:template match="node()[parent::text]
		[contains(normalize-space(), $mezera)]
		[preceding-sibling::node()[1]/self::unclear]
		[contains(normalize-space(preceding-sibling::node()[1]), $mezera)]
		[not(preceding-sibling::node()[1]/parent::*/preceding-sibling::*[1]/self::unclear)]
		"
		priority="0">
			
			
		<xsl:call-template name="process-text-node">
			<xsl:with-param name="current-text">
				<xsl:choose>
					<xsl:when test="following-sibling::node()[1]/self::unclear and
						not(ends-with(., $mezera))">
						<xsl:variable name="temp" select="vwf:substring-after-first-from-many(., $interpunkcePlusMezera)" />
						<xsl:value-of select="concat(vwf:substring-before-last($temp, $mezera), $mezera)"/>
					</xsl:when>
					<xsl:otherwise>
						<xsl:value-of select="vwf:substring-after-first-from-many(., $interpunkcePlusMezera)"/>
					</xsl:otherwise>
				</xsl:choose>
				
			</xsl:with-param>
			<xsl:with-param name="next-unclear">
				<xsl:value-of select="''" />
			</xsl:with-param>
			<xsl:with-param name="previous-unclear">
				<xsl:value-of select="''" />
			</xsl:with-param>
		</xsl:call-template>
			
		
	</xsl:template>

<!-- XXX4 -->
	<xd:doc id="text-after-unclear-with-space">
		<xd:desc>
			<xd:p>Případy, kdy je prvek rekonstrukce/unclear součástí obklopujícího textu; vybere text, který za prvkem rekonstrukce/unclear následují.</xd:p>
		</xd:desc>
	</xd:doc>
	<xsl:template match="text()[not(self::text()[normalize-space()=''])[1]][preceding-sibling::node()[1]/self::unclear][contains(normalize-space(preceding-sibling::node()[1]), $mezera)]
		[not(contains(preceding::node()[not(normalize-space() = '')][self::text()][2], $mezera))]" priority="10">
		
		<!-- count(preceding-sibling::node()[3]) > 0 and count(preceding-sibling::node()[4]) > 0 and -->
		<!-- XXX6 -->
		<xsl:choose>
			<xsl:when test="
				count(preceding-sibling::node()[3]) > 0 and count(preceding-sibling::node()[4]) > 0 and
				not (contains(preceding-sibling::node()[3], $mezera)) and not(contains(preceding-sibling::node()[4], $mezera)) ">
				<xsl:element name="unclear">
					<xsl:element name="supplied">
						<xsl:value-of select="vwf:substring-after-last-from-many(preceding-sibling::node()[1], $interpunkcePlusMezera)"/>
					</xsl:element>
					<xsl:value-of select="vwf:substring-before-first-from-many-including(., $interpunkcePlusMezera)"/>
				</xsl:element>
			</xsl:when>
			
			<xsl:when test="
				preceding-sibling::node()[1][self::unclear]/contains(normalize-space(.), $mezera) and
				not (contains(normalize-space() , $mezera) ) and
				preceding-sibling::node()[2][self::text()]/not (contains(., $mezera)) and
				parent::text/preceding-sibling::unclear[1]/not (contains(., $mezera))
				"
				>
				<!--
					<unclear><supplied>člo</supplied></unclear>
		<text>vě<unclear><supplied>kem n</supplied></unclear>emohl </text>
		<supplied>učiniti. Buoh jsa najvyší </supplied>
				-->
				<xsl:message>nothing</xsl:message>
				<xsl:element name="unclear">
					<xsl:element name="supplied">
						<xsl:value-of select="vwf:substring-after-last-from-many(preceding-sibling::node()[1], $interpunkcePlusMezera)"/>
					</xsl:element>
					<xsl:value-of select="vwf:substring-before-first-from-many-including(., $interpunkcePlusMezera)"/>
				</xsl:element>
			</xsl:when>
			
			
			
			<xsl:when test="preceding-sibling::node()[1][self::unclear]/contains(normalize-space(.), $mezera) and not (contains(normalize-space() , $mezera) )">
				<xsl:message>nothing</xsl:message>
			</xsl:when>

			<xsl:when test="preceding-sibling::node()[2][self::unclear][not (contains(., $mezera))] and preceding-sibling::node()[1][self::text()][not (contains(., $mezera))]">
				<xsl:message>nothing</xsl:message>
			</xsl:when>
			
			<xsl:when test="
				preceding-sibling::node()[4][self::text()]/ends-with(., $mezera)  and
				preceding-sibling::node()[3][self::unclear]/not(contains(., $mezera)) and
				preceding-sibling::node()[2][self::text()]/not(contains(., $mezera)) and
				preceding-sibling::node()[1][self::unclear]/contains(., $mezera)
				">
				<!--
					<supplied>z </supplied><unclear><supplied>dob</supplied></unclear>
					<text>roty buoha <unclear><supplied>ve</supplied></unclear>likého. <unclear>
			<supplied>Velik</supplied></unclear>o<unclear><supplied>st r</supplied></unclear>adosti těch </text>
					
				-->
				<xsl:message>nothing</xsl:message>
			</xsl:when>
			
			
			<xsl:when test="
				preceding-sibling::node()[1]/contains(., $mezera) and
				preceding-sibling::node()[2]/not(contains(., $mezera)) and
				parent::text/preceding-sibling::*[1][self::unclear]/not(contains(., $mezera)) and
				parent::text/preceding-sibling::*[2][self::supplied]/ends-with(., $mezera)
				">
				<!-- 
				<supplied>v </supplied>
				<unclear><supplied>n</supplied></unclear>
				<text>oc<unclear><supplied>i chod</supplied></unclear>iece do chrámu, ženy a děti sebú vo<unclear>
				-->
				<xsl:element name="unclear">
					<xsl:element name="supplied">
						<xsl:value-of select="vwf:substring-after-last-from-many(preceding-sibling::node()[1], $interpunkcePlusMezera)"/>
					</xsl:element>
					<xsl:value-of select="vwf:substring-before-first-from-many-including(., $interpunkcePlusMezera)"/>
				</xsl:element>
			</xsl:when>
			
			
			<xsl:when test="preceding-sibling::node()[1][self::unclear]/contains(normalize-space(.), $mezera) and (contains(normalize-space() , $mezera) ) and
				not(vwf:ends-with(., $mezera)) and
				following-sibling::node()[1]/self::unclear and
				preceding-sibling::node()[4]/self::text()/not(ends-with(., $mezera))">
				<xsl:message>nothing</xsl:message>
			</xsl:when>
			
			<xsl:when test="
				preceding-sibling::node()[1][self::unclear]/contains(normalize-space(.), $mezera) and
				preceding-sibling::node()[2][self::text()]/not(contains(., $mezera)) and
				preceding-sibling::node()[3][self::unclear]/not(contains(normalize-space(.), $mezera))">
				<xsl:message>nothing</xsl:message>
			</xsl:when>
			
			<xsl:when test="
				contains(preceding-sibling::node()[1], $mezera) and
				not(contains(preceding-sibling::node()[2], $mezera)) and
				parent::text/preceding-sibling::supplied[1]/ends-with(., $mezera)
				">
				<!--
					<supplied>člověkem najnižším. A protož Kristus sě řebří </supplied>
		<text>me<unclear><supplied>zi ze</supplied></unclear>mí a ne<unclear><supplied>bem </supplied></unclear></text>
				-->
				<xsl:message>nothing</xsl:message>
			</xsl:when>
			
			<xsl:when test="contains(preceding-sibling::node()[1], $mezera) and not(contains(preceding-sibling::node()[2], $mezera)) ">
				<xsl:element name="unclear">
					<xsl:element name="supplied">
						<xsl:value-of select="vwf:substring-after-last-from-many(preceding-sibling::node()[1], $interpunkcePlusMezera)"/>
					</xsl:element>
					<xsl:value-of select="vwf:substring-before-first-from-many-including(., $interpunkcePlusMezera)"/>
				</xsl:element>
			</xsl:when>

			<xsl:otherwise>
				
			</xsl:otherwise>
		</xsl:choose>
		
		
		<xsl:call-template name="process-text-node">
			<xsl:with-param name="current-text">
				<xsl:choose>
					<xsl:when test="following-sibling::node()[1]/self::unclear and
						not(ends-with(., $mezera))">
						<xsl:variable name="temp" select="vwf:substring-after-first-from-many(., $interpunkcePlusMezera)" />
						<xsl:value-of select="concat(vwf:substring-before-last($temp, $mezera), $mezera)"/>
					</xsl:when>
					<xsl:otherwise>
						<xsl:value-of select="vwf:substring-after-first-from-many(., $interpunkcePlusMezera)"/>
					</xsl:otherwise>
				</xsl:choose>
				
			</xsl:with-param>
			<xsl:with-param name="next-unclear">
				<xsl:value-of select="''" />
			</xsl:with-param>
			<xsl:with-param name="previous-unclear">
				<xsl:value-of select="''" />
			</xsl:with-param>
		</xsl:call-template>
		
		
	</xsl:template>

	<xd:doc id="text-after-unclear">
		<xd:desc>
			<xd:p>Případy, kdy je prvek rekonstrukce/unclear součástí obklopujícího textu; vybere text, který za prvkem rekonstrukce/unclear následují.</xd:p>
		</xd:desc>
	</xd:doc>
	<xsl:template match="text()[not(self::text()[normalize-space()=''])[1]][preceding-sibling::node()[1]/self::unclear]">

		<xsl:call-template name="process-text-node">
			<xsl:with-param name="current-text" select="." />
			<xsl:with-param name="next-unclear">
				<xsl:value-of select="following-sibling::node()[1]/self::unclear/supplied/text()" />
			</xsl:with-param>
			<xsl:with-param name="previous-unclear">
				<xsl:value-of select="preceding-sibling::node()[1]/self::unclear/supplied/text()" />
			</xsl:with-param>
		</xsl:call-template>

	</xsl:template>

	<xd:doc id="text-after-unclear">
		<xd:desc>
			<xd:p>Případy, kdy je prvek rekonstrukce/unclear součástí obklopujícího textu; vybere text, který za prvkem rekonstrukce/unclear následují.</xd:p>
		</xd:desc>
	</xd:doc>
	<xsl:template match="text()[not(self::text()[normalize-space()=''])[1]][preceding-sibling::node()[1]/self::unclear][following-sibling::node()[1]/self::unclear]" priority="5">

		<xsl:call-template name="process-text-node">
			<xsl:with-param name="current-text" select="." />
			<xsl:with-param name="next-unclear">
				<xsl:value-of select="following-sibling::node()[1]/self::unclear/supplied/text()" />
			</xsl:with-param>
			<xsl:with-param name="previous-unclear">
				<xsl:value-of select="preceding-sibling::node()[1]/self::unclear/supplied/text()" />
			</xsl:with-param>
		</xsl:call-template>

	</xsl:template>

	<xsl:template match="text()[1][not(self::text()[normalize-space()=''])[1]][parent::*[1]/preceding-sibling::*[1][self::unclear]]">
		<xsl:call-template name="process-text-node">
			<xsl:with-param name="current-text" select="." />
			<xsl:with-param name="next-unclear">
				<xsl:value-of select="parent::*[1]/following-sibling::*[1]/text()[1]" />
			</xsl:with-param>
			<xsl:with-param name="previous-unclear">
				<xsl:if test="not(parent::*[1]/preceding-sibling::node()[1][not(normalize-space()='')][self::text()])">
				<xsl:value-of select="parent::*[1]/preceding-sibling::*[1]/self::unclear/supplied/text()" />
				</xsl:if>
			</xsl:with-param>
		</xsl:call-template>
	</xsl:template>


	<xsl:template match="corr | sic | note[not(//unclear)]">
		<xsl:copy-of select="." />
	</xsl:template>

	<xsl:template name="process-unclear">
		<xsl:param name="current-text" />
		<xsl:param name="source-previous-text" as="node()" />
		<xsl:param name="source-next-text"  />
		<xsl:param name="previous-node" as="node()" />
		<xsl:param name="next-node" as="node()" />
		<xsl:param name="source-next-next-node" as="node()" />
		<xsl:param name="source-previous-previous-text" />

		<xsl:variable name="result-previous-text">
			<xsl:choose>
				<xsl:when test="vwf:ends-with-one-of-character($source-previous-text, $interpunkcePlusMezera)">
					<xsl:value-of select="''" />
				</xsl:when>
				<xsl:otherwise>
					<xsl:variable name="prev-text" select="string($source-previous-text)" />
					<xsl:choose>
						<xsl:when test="contains($prev-text, $mezera)">
							<!--<xsl:value-of select="vwf:substring-after-last-from-many($prev-text, $mezera)" />-->
							<!-- XXX1 -->
							<xsl:value-of select="vwf:substring-after-last-from-many($prev-text, $interpunkcePlusMezera)" />
						</xsl:when>
						<xsl:otherwise>
							<xsl:choose>
								<xsl:when test="string($previous-node) != ''">
									<xsl:choose>
										<xsl:when test=" vwf:ends-with($previous-node, $mezera)">
											<xsl:value-of select="$prev-text" />
										</xsl:when>
										<xsl:otherwise>
											<xsl:value-of select="''" />
										</xsl:otherwise>
									</xsl:choose>
								</xsl:when>
								<xsl:otherwise>
									<xsl:value-of select="$prev-text" />
								</xsl:otherwise>
							</xsl:choose>
						</xsl:otherwise>
					</xsl:choose>
				</xsl:otherwise>
			</xsl:choose>
		</xsl:variable>

		<xsl:variable name="result-next-text">
			<xsl:choose>
				<xsl:when test="ends-with($current-text, $mezera)">
					<xsl:value-of select="''" />
				</xsl:when>
				<xsl:otherwise>
					<xsl:choose>
						<xsl:when test="vwf:starts-with-one-from-many($source-next-text, $interpunkcePlusMezera)">
							<xsl:value-of select="''" />
						</xsl:when>
						<xsl:otherwise>
							<xsl:variable name="znak">
								<xsl:call-template name="najitPrvniVyskytujiciSeZnak">
									<xsl:with-param name="text" select="$source-next-text" />
									<xsl:with-param name="interpunkce" select="$interpunkcePlusMezera" />
								</xsl:call-template>
							</xsl:variable>
							<xsl:choose>
								<xsl:when test="$znak = ''">
									<xsl:value-of select="$source-next-text" />
								</xsl:when>
								<xsl:otherwise>
									<xsl:value-of select="substring-before($source-next-text, $znak)" />
								</xsl:otherwise>
							</xsl:choose>
						</xsl:otherwise>
					</xsl:choose>
				</xsl:otherwise>
			</xsl:choose>
		</xsl:variable>

		<xsl:variable name="next-text-delimiter">
			<xsl:call-template name="najitPrvniVyskytujiciSeZnak">
				<xsl:with-param name="text" select="$source-next-text" />
				<xsl:with-param name="interpunkce" select="$interpunkcePlusMezera" />
			</xsl:call-template>
		</xsl:variable>

		<xsl:variable name="result-next-next-supplied">
			<xsl:choose>
				<xsl:when test="string($next-node) != ''">
					<xsl:choose>
						<xsl:when test="not(starts-with($next-node, $mezera)) and contains(normalize-space($next-node), $mezera) ">
							<xsl:value-of select="substring-before($next-node, $mezera)" />
						</xsl:when>
						<xsl:when test="vwf:starts-with-one-from-many($next-node, $interpunkcePlusMezera)">
							<xsl:value-of select="''" />
						</xsl:when>
						<xsl:when test="$next-node = $source-next-text">
							<xsl:value-of select="''" />
						</xsl:when>
						<xsl:otherwise>
							<xsl:value-of select="$next-node" />
						</xsl:otherwise>
					</xsl:choose>
				</xsl:when>
				<xsl:otherwise>
					<xsl:value-of select="''" />
				</xsl:otherwise>
			</xsl:choose>

		</xsl:variable>

	<xsl:variable name="result-next-next-supplied">
		<xsl:choose>
			<xsl:when test="$next-text-delimiter != ''">
				<xsl:value-of select="''" />
			</xsl:when>
			<xsl:when test="contains($current-text, $mezera)">
				<xsl:value-of select="''" />
			</xsl:when>
			<xsl:when test="string($source-next-next-node) = ''">
				<xsl:value-of select="''" />
			</xsl:when>
			<xsl:when test="vwf:starts-with-one-from-many($source-next-next-node, $interpunkcePlusMezera)">
				<xsl:value-of select="''" />
			</xsl:when>
			<xsl:when test="not(starts-with($next-node, $mezera)) and contains(normalize-space($source-next-next-node), $mezera)">
				<xsl:value-of select="substring-before($source-next-next-node, $mezera)" />
			</xsl:when>
			<xsl:when test="vwf:ends-with-one-of-character($source-next-next-node, $interpunkcePlusMezera)">
				<xsl:variable name="znak">
					<xsl:call-template name="najitPosledniVyskytujiciSeZnak">
						<xsl:with-param name="text" select="$source-next-next-node" />
						<xsl:with-param name="interpunkce" select="$interpunkcePlusMezera" />
					</xsl:call-template>
				</xsl:variable>
				<xsl:value-of select="vwf:substring-before-last($source-next-next-node, $znak)" />
			</xsl:when>
			<xsl:otherwise>
				<xsl:value-of select="$source-next-next-node" />
			</xsl:otherwise>
		</xsl:choose>
	</xsl:variable>

		<xsl:choose>
			<xsl:when test="$current-text = ''"></xsl:when>
			<xsl:when test="starts-with($current-text, $mezera)">
				<xsl:copy>
					<xsl:copy-of select="@*" />
					<supplied>
						<xsl:value-of select="substring-after($current-text, $mezera)" />
					</supplied>
					<xsl:value-of select="$result-next-text" />
					<xsl:if test="$result-next-text != '' and $next-text-delimiter = $mezera">
						<xsl:value-of select="$mezera" />
					</xsl:if>
				</xsl:copy>
			</xsl:when>
			<xsl:when test="contains(normalize-space($current-text), $mezera)">
				<xsl:if test="$result-previous-text != ''">
					<xsl:copy>
						<xsl:copy-of select="@*" />
						<xsl:value-of select="$result-previous-text" />
						<supplied>
							<xsl:value-of select="concat(substring-before($current-text, $mezera), $mezera)" />
						</supplied>
					</xsl:copy>
				</xsl:if>
				<xsl:copy>
					<xsl:copy-of select="@*" />
					<supplied>
						<xsl:value-of select="substring-after($current-text, $mezera)" />
					</supplied>
					<xsl:value-of select="$result-next-text" />
					<xsl:if test="$result-next-text != '' and $next-text-delimiter = $mezera">
						<xsl:value-of select="$mezera" />
					</xsl:if>
				</xsl:copy>
			</xsl:when>
			<xsl:otherwise>
				<xsl:copy>
					<xsl:copy-of select="@*" />
					<xsl:if test="$source-previous-previous-text">
						<xsl:value-of select="$source-previous-previous-text"/>
					</xsl:if>
					<!-- pokud předchází prázdný element (pb, cb) -->
					<xsl:if test="parent::*[1]/preceding-sibling::*[1][not(node())][not(@rend='space')][1] and
						not(contains( preceding-sibling::node()[1],$mezera))">
						<xsl:copy-of select="parent::*[1]/preceding-sibling::*[1][not(node())][not(@rend='space')][1]"/>
					</xsl:if>
						<xsl:value-of select="$result-previous-text" />
					<!-- pokud předchází prázdný element (pb, cb) -->
					<xsl:if test="preceding-sibling::*[1][not(node())][1]">
						<xsl:copy-of select="preceding-sibling::*[1][not(node())][1]"/>
					</xsl:if>
					<!-- pokud předchází prázdný element (pb, cb) -->
					<xsl:copy-of select="supplied" />
					<!-- pokud následuje prázdný element (pb, cb) -->
					<xsl:if test="following-sibling::*[1][not(node())][1]">
						<xsl:copy-of select="following-sibling::*[1][not(node())][1]"/>
					</xsl:if>
					<!-- pokud následuje prázdný element (pb, cb) -->
					<xsl:value-of select="$result-next-text" />
					<xsl:if test="$result-next-text != '' and $next-text-delimiter = $mezera">
						<xsl:value-of select="$mezera" />
					</xsl:if>
					<xsl:if test="$result-next-next-supplied != '' and $result-next-next-supplied != $result-next-text">
						<xsl:element name="supplied">
							<xsl:value-of select="$result-next-next-supplied" />
							<xsl:if test="starts-with($source-next-next-node, concat($result-next-next-supplied, $mezera))">
								<xsl:value-of select="$mezera" />
							</xsl:if>
						</xsl:element>
					</xsl:if>
				</xsl:copy>
			</xsl:otherwise>
		</xsl:choose>

	</xsl:template>

	<xsl:template name="process-text-node">
		<xsl:param name="current-text" />
		<xsl:param name="previous-unclear" />
		<xsl:param name="next-unclear" />

		<xsl:variable name="start" as="xs:integer">
			<xsl:choose>
				<xsl:when test="string-length($previous-unclear) = 0">
					<xsl:value-of select="1" />
				</xsl:when>
				<xsl:when test="vwf:ends-with($previous-unclear, $mezera)">
					<xsl:value-of select="1" />
				</xsl:when>
				<xsl:otherwise>
					<xsl:value-of select="vwf:first-position-of-many($current-text, $interpunkcePlusMezera)" />
				</xsl:otherwise>
			</xsl:choose>
		</xsl:variable>

		<xsl:variable name="end" as="xs:integer">
			<xsl:choose>
				<xsl:when test="$next-unclear = ''">
					<xsl:value-of select="string-length($current-text)" />
				</xsl:when>
				<xsl:otherwise>
					<xsl:value-of select="vwf:last-position-of-many($current-text, $interpunkcePlusMezera)" />
				</xsl:otherwise>
			</xsl:choose>
		</xsl:variable>

		<xsl:variable name="length" as="xs:integer">
			<xsl:value-of select="$end + 1 - $start" />
		</xsl:variable>

		<xsl:variable name="substring">
			<xsl:choose>
				<xsl:when test="$start > 0">
					<xsl:value-of select="substring($current-text, $start, $length)" />		
				</xsl:when>
				<xsl:otherwise>
					<xsl:value-of select="''"/>
				</xsl:otherwise>
			</xsl:choose>
			
		</xsl:variable>

		<xsl:message>
			<xsl:value-of select="$previous-unclear" />|<xsl:value-of select="$current-text" />|<xsl:value-of select="$next-unclear" />|<xsl:value-of select="$substring" />
		</xsl:message>

		<xsl:value-of select="$substring" />

	</xsl:template>



</xsl:stylesheet>
