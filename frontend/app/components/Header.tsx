import { motion } from "motion/react";
import { BiSolidVideo } from "react-icons/bi";
import { IoIosSettings } from "react-icons/io";

export default function Header() {
        return (
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
        );
}
