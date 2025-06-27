import { Tab } from "@headlessui/react";
import { classNames } from "./ManagePage";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faCircle } from "@fortawesome/free-solid-svg-icons";
import LivePhase from "../components/view/LivePhase";
import TournamentSettings from "../components/manage/tournament/TournamentSettings";

export default function ViewPage() {
  return (
    <div>
      <h1 className="text-3xl text-center">In The Groove Eurocup 2025</h1>
      <Tab.Group>
        <Tab.List className="flex flex-row gap-10 border-b mt-5">
          <Tab
            className={({ selected }) =>
              classNames(
                "py-2 px-4 text-lg",
                selected
                  ? "border-b-2 border-blue-500 font-bold text-blue-500"
                  : "text-gray-500"
              )
            }
          >
            <div className="flex flex-row gap-3 items-center">
              <FontAwesomeIcon
                icon={faCircle}
                className="text-red-500 text-sm animate-pulse"
              />
              <span>LIVE</span>
            </div>
          </Tab>
          <Tab
            className={({ selected }) =>
              classNames(
                "py-2 px-4 text-lg",
                selected
                  ? "border-b-2 border-blue-500 font-bold text-blue-500"
                  : "text-gray-500"
              )
            }
          >
            History
          </Tab>
        </Tab.List>
        <Tab.Panels className="mt-3">
          <Tab.Panel><LivePhase /></Tab.Panel>
          <Tab.Panel><TournamentSettings controls={false}/></Tab.Panel>
        </Tab.Panels>
      </Tab.Group>
    </div>
  );
}
