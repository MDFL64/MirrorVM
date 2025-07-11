use std::collections::HashMap;

mod prospero;
mod physics;

const TEXT: &str = r#"
The Napoleonic Wars (1803–1815) were a series of conflicts fought between the French First Republic (1803–1804) and First French Empire (1804–1815) under the First Consul and Emperor of the French, Napoleon Bonaparte, and a fluctuating array of European coalitions. The wars originated in political forces arising from the French Revolution (1789–1799) and from the French Revolutionary Wars (1792–1802) and produced a period of French domination over Continental Europe.[31] The wars are categorised as seven conflicts, five named after the coalitions that fought Napoleon, plus two named for their respective theatres: the War of the Third Coalition, War of the Fourth Coalition, War of the Fifth Coalition, War of the Sixth Coalition, War of the Seventh Coalition, the Peninsular War, and the French invasion of Russia.[32]

The first stage of the war broke out when Britain declared war on France on 18 May 1803, alongside the Third Coalition. In December 1805, Napoleon I defeated the allied Russo-Austrian army at Austerlitz, which defeated the Holy Roman Empire and thus forced Austria to make peace. Concerned about increasing French power, Prussia led the creation of the Fourth Coalition, which resumed war in October 1806. Napoleon soon defeated the Prussians at Jena-Auerstedt and the Russians at Friedland, bringing an uneasy peace to the continent. The treaty had failed to end the tension, and war broke out again in 1809, with the Austrian-led Fifth Coalition. At first, the Austrians won a significant victory at Aspern-Essling but were quickly defeated at Wagram.

Hoping to isolate and weaken Britain economically through his Continental System, Napoleon launched an invasion of Portugal, the only remaining British ally in continental Europe. After occupying Lisbon in November 1807, and with the bulk of French troops present in Spain, Napoleon seized the opportunity to turn against his former ally, depose the reigning Spanish royal family, and declare his brother King of Spain in 1808 as Joseph I. The Spanish and Portuguese thus revolted, with British support, and expelled the French from Iberia in 1814 after six years of fighting.

Concurrently, Russia, unwilling to bear the economic consequences of reduced trade, routinely violated the Continental System, prompting Napoleon I to launch a massive invasion of Russia in 1812. The resulting campaign ended in disaster for France and the near-destruction of Napoleon's Grande Armée.

Encouraged by the defeat, Great Britain, Austria, Prussia, Sweden, and Russia formed the Sixth Coalition and began a new campaign against France, decisively defeating Napoleon at Leipzig in October 1813. The Allies then invaded France from the east, while the Peninsular War spilled over into southwestern France. Coalition troops captured Paris at the end of March 1814, forced Napoleon to abdicate in April, exiled him to the island of Elba, and restored power to the Bourbons. Napoleon escaped in February 1815 and reassumed control of France for around one Hundred Days. The allies formed the Seventh Coalition, which defeated him at Waterloo in June 1815, and exiled him to the island of Saint Helena, where he died six years later in 1821.[33]

The wars had profound consequences on global history, including the spread of nationalism and liberalism, advancements in civil law, the rise of Britain as the world's foremost naval and economic power, the appearance of independence movements in Spanish America and the subsequent decline of the Spanish and Portuguese Empires, the fundamental reorganization of German and Italian territories into larger states, and the introduction of radically new methods of conducting warfare. After the end of the Napoleonic Wars, the Congress of Vienna redrew Europe's borders and brought a relative peace to the continent, lasting until the Revolutions of 1848 and the Crimean War in 1853.

Napoleon seized power in 1799, establishing a military dictatorship.[34] There are numerous opinions on the date to use as the formal beginning of the Napoleonic Wars; 18 May 1803 is often used, when Britain and France ended the only short period of peace between 1792 and 1814.[35] The Napoleonic Wars began with the War of the Third Coalition, which was the first of the Coalition Wars against the First French Republic after Napoleon's accession as leader of France.

Britain ended the Treaty of Amiens, declaring war on France in May 1803. Among the reasons were Napoleon's changes to the international system in Western Europe, especially in Switzerland, Germany, Italy, and the Netherlands. Historian Frederick Kagan argues that Britain was irritated in particular by Napoleon's assertion of control over Switzerland. Furthermore, Britons felt insulted when Napoleon stated that their country deserved no voice in European affairs, even though King George III was an elector of the Holy Roman Empire. For its part, Russia decided that the intervention in Switzerland indicated that Napoleon was not looking toward a peaceful resolution of his differences with the other European powers.[35]

The British hastily enforced a naval blockade of France to starve it of resources. Napoleon responded with economic embargoes against Britain, and sought to eliminate Britain's Continental allies to break the coalitions arrayed against him. The so-called Continental System formed a League of Armed Neutrality to disrupt the blockade and enforce free trade with France. The British responded by capturing the Danish fleet, breaking up the league, and later secured dominance over the seas, allowing it to freely continue its strategy.

Napoleon won the War of the Third Coalition at Austerlitz, forcing the Austrian Empire out of the war, and formally dissolving the Holy Roman Empire. Within months, Prussia declared war, triggering a War of the Fourth Coalition. This war ended disastrously for Prussia, which had been defeated and occupied within 19 days of the beginning of the campaign. Napoleon subsequently defeated Russia at Friedland, creating powerful client states in Eastern Europe and ending the Fourth Coalition.

Concurrently, the refusal of Portugal to commit to the Continental System, and Spain's failure to maintain it, led to the Peninsular War and the outbreak of the War of the Fifth Coalition. The French occupied Spain and formed a Spanish client kingdom, ending the alliance between the two. Heavy British involvement in the Iberian Peninsula soon followed, while a British effort to capture Antwerp failed. Napoleon oversaw the situation in Iberia, defeating the Spanish, and expelling the British from the Peninsula. Austria, eager to recover territory lost during the War of the Third Coalition, invaded France's client states in Eastern Europe in April 1809. Napoleon defeated the Fifth Coalition at Wagram.

Plans to invade British North America pushed the United States to declare war on Britain in the War of 1812, but it did not become an ally of France. Grievances over control of Poland, and Russia's withdrawal from the Continental System, led to Napoleon invading Russia in June 1812. The invasion was an unmitigated disaster for Napoleon; scorched earth tactics, desertion, French strategic failures and the onset of the Russian winter compelled Napoleon to retreat with massive losses. Napoleon suffered further setbacks: French power in the Iberian Peninsula was broken at the Battle of Vitoria the following summer, and a new alliance began, the War of the Sixth Coalition.

The coalition defeated Napoleon at Leipzig, precipitating his fall from power and eventual abdication on 6 April 1814. The victors exiled Napoleon to Elba and restored the Bourbon monarchy. Napoleon escaped from Elba in 1815, gathering enough support to overthrow the monarchy of Louis XVIII, triggering a seventh, and final, coalition against him. Napoleon was then decisively defeated at Waterloo, and he abdicated again on 22 June. On 15 July, he surrendered to the British at Rochefort, and was permanently exiled to remote Saint Helena. The Treaty of Paris, signed on 20 November 1815, formally ended the war.

The Bourbon monarchy was once again restored, and the victors began the Congress of Vienna to restore peace to Europe. As a direct result of the war, the Kingdom of Prussia rose to become a great power,[36] while Great Britain, with its unequalled Royal Navy and growing Empire, became the world's dominant superpower, beginning the Pax Britannica.[37] The Holy Roman Empire had been dissolved, and the philosophy of nationalism that emerged early in the war contributed greatly to the later unification of the German states, and those of the Italian peninsula. The war in Iberia greatly weakened Spanish power, and the Spanish Empire began to unravel; Spain would lose nearly all of its American possessions by 1833. The Portuguese Empire also shrank, with Brazil declaring independence in 1822.[38]

The wars revolutionised European warfare; the application of mass conscription and total war led to campaigns of unprecedented scale, as whole nations committed all their economic and industrial resources to a collective war effort.[39] Tactically, the French Army had redefined the role of artillery, while Napoleon emphasised mobility to offset numerical disadvantages,[40] and aerial surveillance was used for the first time in warfare.[41] The highly successful Spanish guerrillas demonstrated the capability of a people driven by fervent nationalism against an occupying force.[42][page range too broad] Due to the longevity of the wars, the extent of Napoleon's conquests, and the popularity of the ideals of the French Revolution, the period had a deep impact on European social culture. Many subsequent revolutions, such as that of Russia, looked to the French as a source of inspiration,[43] while its core founding tenets greatly expanded the arena of human rights and shaped modern political philosophies in use today.[44]
"#;

const JSON: &str = include_str!("data.json");

#[no_mangle]
pub extern "C" fn bench_regex() -> i32 {
    use regex::Regex;

    fn inner() {
        let mut result = 0;
        {
            let re = Regex::new(r"18[0-9]{2}").unwrap();
            for m in re.find_iter(TEXT) {
                result += m.as_str().parse::<i32>().unwrap();
            }
        }
        {
            let re = Regex::new(r#""@+""#).unwrap();
            for m in re.find_iter(JSON) {
                result += m.len() as i32;
            }
        }
        assert_eq!(result,66976);
    }
    for _ in 0..250 {
        inner();
    }

    0
}

#[no_mangle]
pub extern "C" fn bench_rand_sort() -> i32 {
    use rand::{Rng, SeedableRng};
    let mut random = rand::rngs::SmallRng::seed_from_u64(0x50312A88_BC00F213);
    let mut vec = Vec::<f64>::new();
    for i in 0..2_000_000 {
        vec.push(random.random::<f64>() * 1_000_000_000.0);
    }
    vec.sort_by(|a,b| a.partial_cmp(b).unwrap());

    let result = vec[vec.len()/2] as i32;
    // do not assert: this is platform dependant
    //assert_eq!(result, 500520631);
    result
}

#[no_mangle]
pub extern "C" fn bench_hashes() -> i32 {
    assert_eq!(hash_md5(), 2148500);
    assert_eq!(hash_sha1(), 18466000);
    assert_eq!(hash_sha2(), 9989000);
    assert_eq!(hash_sha3(), 10315000);
    12345
}

#[no_mangle]
pub extern "C" fn bench_json() -> i32 {
    fn inner() {
        let mut scores = HashMap::<String,f64>::new();
        use serde_json::Value;
        let v: Value = serde_json::from_str(JSON).unwrap();
        let Value::Array(array) = v else { panic!() };
        for item in array {
            let Value::Object(obj) = item else { panic!() };
            for (key,value) in obj {
                let entry = scores.entry(key).or_default();
                match value {
                    Value::Number(n) => *entry += n.as_f64().unwrap(),
                    Value::String(s) => *entry += s.len() as f64,
                    Value::Bool(true) => *entry *= 1.1,
                    Value::Bool(false) => *entry *= 0.9,
                    _ => ()
                }
            }
        }
    
        let mut result = 0;
        for (_,n) in scores {
            result += n as i32;
        }
        assert_eq!(result,4940);
    }

    for _ in 0..100 {
        inner();
    }

    0
}

#[no_mangle]
pub extern "C" fn bench_zip() -> i32 {
    use flate2::write::{GzEncoder, GzDecoder};
    use flate2::Compression;
    use std::io::prelude::*;

    fn compress(input: &[u8]) -> i32 {
        let mut encoder = GzEncoder::new(Vec::new(), Compression::best());
        encoder.write_all(input).unwrap();
        let output = encoder.finish().unwrap();

        let mut decoder = GzDecoder::new(Vec::new());
        decoder.write_all(&output).unwrap();
        let restored = decoder.finish().unwrap();

        assert_eq!(input,restored);

        output.len() as i32
    }

    for _ in 0..20 {
        let size = compress(TEXT.as_bytes()) + compress(JSON.as_bytes());
        assert_eq!(size,32905);
    }
    0
}

#[no_mangle]
pub extern "C" fn bench_image() -> i32 {
    use image::{DynamicImage, ImageReader, ExtendedColorType, ImageEncoder};
    use image::codecs::png::PngEncoder;
    use std::io::Cursor;
    let img = ImageReader::new(Cursor::new(include_bytes!("nyc.jpg")))
        .with_guessed_format().unwrap().decode().unwrap();
    
    let DynamicImage::ImageRgb8(rgb) = img else { panic!("bruh" )};
    assert_eq!(rgb.width(), 1500);
    assert_eq!(rgb.height(), 1166);

    let mut png_buffer = Vec::new();

    let encoder = PngEncoder::new(&mut png_buffer);
    encoder.write_image(&rgb, rgb.width(), rgb.height(), ExtendedColorType::Rgb8).unwrap();

    assert_eq!(png_buffer.len(),3005730);
    0
}

fn hash_md5() -> i32 {
    let mut res = 0i32;
    
    for y in 0..1000 {
        let hash = md5::compute(TEXT);
        for x in hash.0 {
            res += x as i32;
        }
        res += y;
    }

    res
}

fn hash_sha1() -> i32 {
    use sha1::Digest;

    let mut res = 0i32;
    
    for y in 0..4000 {
        let mut hash = sha1::Sha1::new();
        hash.update(TEXT);
        let bytes= hash.finalize();
        for x in bytes {
            res += x as i32;
        }
        res += y;
    }

    res
}

fn hash_sha2() -> i32 {
    use sha2::Digest;

    let mut res = 0i32;
    
    for y in 0..2000 {
        let mut hash = sha2::Sha256::new();
        hash.update(TEXT);
        let bytes= hash.finalize();
        for x in bytes {
            res += x as i32;
        }
        res += y;
    }

    res
}

fn hash_sha3() -> i32 {
    use sha3::Digest;

    let mut res = 0i32;
    
    for y in 0..2000 {
        let mut hash = sha3::Sha3_256::new();
        hash.update(TEXT);
        let bytes= hash.finalize();
        for x in bytes {
            res += x as i32;
        }
        res += y;
    }

    res
}
