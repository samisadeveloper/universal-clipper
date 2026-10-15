import type { Route } from "./+types/library";
import { isDaemonRunning, useClips, type Clip } from "../ClipLoader";
import ClipPreview from "../components/ClipPreview";

export function meta({}: Route.MetaArgs) {
  return [
    { title: "Viewing clips in the library" },
    { name: "description", content: "Download the client to manage clips" },
  ];
}

import { BiSolidVideo } from "react-icons/bi";
import { IoIosSettings } from "react-icons/io";
import { motion } from "motion/react";
import { useEffect, useState } from "react";

const secondaryText = '#b6b6b8';

function LibraryView({ clips }: {clips: Clip[]}) {
        return (
                <div style={{display: "grid", gridTemplateColumns: "auto auto auto"}}>
                        {clips.map(clip => (
                                <ClipPreview key={clip.id} clip={clip} />
                        ))}
                </div>
        );
}

export default function Library() {
        const { clips } = useClips();

        const [isRunning, setRunning] = useState(false);

        useEffect(() => {
                async function getStatus() {
                        const running = await isDaemonRunning();

                        setRunning(running);
                }

                getStatus();
        });

        return (
                <div>
                        <div style={{backgroundColor: '#1f212b', width: '100%', height: 50, display: 'flex', gap: 20}}>
                                <motion.div 
                                whileHover={{scale: 1.1}}

                                style={{cursor: 'pointer', marginLeft: 20, display: 'flex', alignItems: 'center', gap: 5}}
                                >
                                        <BiSolidVideo />
                                        <p>View Library</p>
                                </motion.div>

                                <motion.div 
                                whileHover={{scale: 1.1}}

                                style={{cursor: 'pointer', display: 'flex', alignItems: 'center', gap: 5}}
                                >
                                        <IoIosSettings />
                                        <p>Settings</p>
                                </motion.div>
                        </div>

                        <div style={{marginRight: 15, marginLeft: 15}}>
                                {clips.length > 0 && <LibraryView clips={clips}/>}
                                {clips.length < 1 && 
                                        <div style={{alignContent: 'center', justifyItems: 'center', height: '100vh'}}>
                                                <div style={{justifyItems: 'center', width: 1000, height: 500}}>
                                                        <h1 style={{marginTop: 20, fontSize: 55}}>Waiting for the magic to happen</h1>
                                                        <p style={{color: secondaryText, fontSize: 35}}>{isRunning ? "No clips detected so far." : "No clipper client running."}</p>
                                                </div>
                                        </div>
                                }
                        </div>
                </div>
        );
}
