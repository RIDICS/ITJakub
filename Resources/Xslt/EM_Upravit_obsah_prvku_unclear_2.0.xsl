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
	<xsl:template match="unclear[preceding-sibling::node()[not(self::text()[normalize-space()=''])][1][self::*]]" priority="5">

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
				<xsl:value-of select="parent::*[1]/text()[1]" />
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
			 	<xsl:otherwise>
			 		<xsl:value-of select="following-sibling::node()[2]
			 			[not(self::text()[normalize-space()=''])[1]]" />
			 	</xsl:otherwise>
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
				<xsl:value-of select="preceding-sibling::*[1]/text()[last()]" />
			</xsl:with-param>

			<xsl:with-param name="source-next-text">
				<xsl:value-of select="following-sibling::*[1]/text()[1]" />
			</xsl:with-param>
			
			<xsl:with-param name="previous-node">
				<xsl:value-of select="preceding-sibling::node()[1]" />
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
				<xsl:value-of select="parent::*[1]/preceding-sibling::*[1]/self::unclear/supplied/text()" />
			</xsl:with-param>
		</xsl:call-template>
	</xsl:template>


	<xsl:template match="corr | sic | note">
		<xsl:copy-of select="." />
	</xsl:template>

	<xsl:template name="process-unclear">
		<xsl:param name="current-text" />
		<xsl:param name="source-previous-text" as="node()" />
		<xsl:param name="source-next-text"  />
		<xsl:param name="previous-node" as="node()" />
		<xsl:param name="next-node" as="node()" />
		<xsl:param name="source-next-next-node" as="node()" />

		<xsl:variable name="result-previous-text">
			<xsl:choose>
				<xsl:when test="vwf:ends-with-one-of-character($source-previous-text, $interpunkcePlusMezera)">
					<xsl:value-of select="''" />
				</xsl:when>
				<xsl:otherwise>
					<xsl:variable name="prev-text" select="string($source-previous-text)" />
					<xsl:choose>
						<xsl:when test="contains($prev-text, $mezera)">
							<xsl:value-of select="vwf:substring-after-last-from-many($prev-text, $mezera)" />
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
					<xsl:value-of select="$result-previous-text" />
					<xsl:copy-of select="supplied" />
					<xsl:value-of select="$result-next-text" />
					<xsl:if test="$result-next-text != '' and $next-text-delimiter = $mezera">
						<xsl:value-of select="$mezera" />
					</xsl:if>
					<xsl:if test="$result-next-next-supplied != ''">
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
			<xsl:value-of select="substring($current-text, $start, $length)" />
		</xsl:variable>

		<xsl:message>
			<xsl:value-of select="$previous-unclear" />|<xsl:value-of select="$current-text" />|<xsl:value-of select="$next-unclear" />|<xsl:value-of select="$substring" />
		</xsl:message>

		<xsl:value-of select="$substring" />

	</xsl:template>



</xsl:stylesheet>
